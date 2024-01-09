module Ares.Compiler.Parser.Internal.MemberParser

open Ares.Compiler.Parser.Internal.ExpressionParser.TypeDescriptorParsers
open Ares.Compiler.Parser.Syntax.Member
open Ares.Compiler.Parser.Internal.ParserUtils
open Ares.Compiler.Parser.Internal.CommonParsers
open Ares.Compiler.Parser.Internal.ExpressionParser
open Ares.Compiler.Parser.Internal.TypeParamParser
open Ares.Compiler.Parser.Internal.TypeConstraintParser
open FParsec

let scopePrivateP = skipString "private" |>> fun () -> Scope.Private
let scopeProtectedP = skipString "protected" |>> fun () -> Scope.Protected
let scopeInternalP = skipString "internal" |>> fun () -> Scope.Internal
let scopePublicP = skipString "public" |>> fun () -> Scope.Public
let public scopeParser = choice [ scopePrivateP; scopeProtectedP; scopeInternalP; scopePublicP; ]


module internal FunctionMemberParsers =
    
    module internal GivenClauseParsers =
        let clauseStartP = pstring "given" .>> skipChar ':' .>> spaces
        let givenExp =
            (typeArgumentIdParser .>> spaces) .>>. (typeConstraintParser .>> spaces .>> skipChar ';')
        let constraintExpsP = (many givenExp)
        let public givenClauseParser = clauseStartP >>. constraintExpsP
    let methodScopeP = spaces >>. scopeParser .>> spaces
    let inferredReturnTypeP = (spaces >>. skipString "func" .>> spaces) |>> fun _ -> FunctionReturnType.Inferred
    let specificReturnTypeP = (spaces >>. typeDescriptorParser .>> spaces) |>> FunctionReturnType.TypeDescriptor
    let returnTypeP = inferredReturnTypeP <|> specificReturnTypeP
    let memberNameP = simpleIdStringParser
    let typeParamsP = TypeParamParsers.typeParamsParser
    let argParser = (typeDescriptorParser .>> spaces) .>>. (simpleIdStringParser)
    let methodParamsP =
        createCharDelimitedExpressionParser '(' ')' ',' argParser
    let signatureMapper = fun (typeConstraints, scope, returnType, funcName, typeParams, parameters) -> {
                Name = funcName;
                Scope = scope;
                TypeParameters = typeParams;
                Parameters = parameters;
                ReturnType = returnType;
                TypeConstraints = typeConstraints;
            }
    let optionalTcMapParser =
        let fallback = preturn []
        let finalP = GivenClauseParsers.givenClauseParser <|> fallback
        finalP
    let signatureP =
        (optionalTcMapParser .>>.
        (pipe5
            methodScopeP
            returnTypeP 
            memberNameP 
            (attempt typeParamsP <|> preturn [])
            methodParamsP
            (fun s rt n tp mp ->
                fun tcs -> signatureMapper (tcs, s, rt, n, tp, mp)))) |>>
            fun (tcs, makeMapper) ->
                makeMapper tcs
                
    let openB = spaces >>. skipChar '{' >>. spaces
    let closeB = spaces >>. skipChar '}' >>. spaces
    let bodyStmtP = between openB closeB StatementParser.statementParser
    let public functionMemberParser =
        getGpsSpan (pipe2
            signatureP
            bodyStmtP
            (fun s b -> Function(s, b))) |>> MemberSyntaxElement

module internal TypeDescriptorMemberParsers =
    let scopeP = spaces >>. scopeParser .>> spaces
    let typeP = skipString "type" .>> spaces
    let genericTypeDeclMapper = fun firstTp restTp -> firstTp :: restTp
    let typeParamListParser = sepBy1 typeParamParser (spaces >>. skipChar ',' .>> spaces)
    let typeAssignmentEqualP = spaces .>> skipChar '=' .>> spaces
    
    let nonGenericTypeDeclP =
        typeP >>. simpleUppercaseIdStringParser .>> typeAssignmentEqualP .>>. typeDescriptorParser |>>
                             fun (id, td) ->
                                 fun scope -> TypeDeclaration(scope, id, [], td)

    let genericTypeDeclP =
        pipe3
            (typeP >>. simpleUppercaseIdStringParser)
            TypeParamParsers.typeParamsParser
            (skipChar '=' >>. spaces >>. typeDescriptorParser)
            (fun id tps td ->
                fun scope -> TypeDeclaration(scope, id, tps, td))
    let typeDeclNoScopeP =
        pipe3
            (typeP >>. simpleUppercaseIdStringParser)
            (attempt TypeParamParsers.typeParamsParser <|> preturn [])
            (skipChar '=' >>. spaces >>. typeDescriptorParser)
            (fun id tps td ->
                fun scope -> TypeDeclaration(scope, id, tps, td))
    let sepP = skipChar ';'
    let typeDeclEither =
        scopeP .>>.
        choice [ attempt genericTypeDeclP; nonGenericTypeDeclP; ] .>>
        sepP |>> fun (s, m) -> m s
    let typeDeclarationMemberParser =
        getGpsSpan typeDeclEither |>> MemberSyntaxElement

module internal ScratchMemberParsers =
    let keywordP = skipString "scratch" >>. spaces
    let openB = spaces >>. skipChar '{' >>. spaces
    let closeB = spaces >>. skipChar '}' >>. spaces
    let bodyStmtP = between openB closeB StatementParser.statementParser
    let scratchParser =
        keywordP >>. bodyStmtP |>> Scratch
    let public scratchMemberParser =
        getGpsSpan (keywordP >>. bodyStmtP |>> Scratch) |>> MemberSyntaxElement

let memberParser = choice [
    attempt FunctionMemberParsers.functionMemberParser
    attempt TypeDescriptorMemberParsers.typeDeclarationMemberParser
    ScratchMemberParsers.scratchMemberParser
]