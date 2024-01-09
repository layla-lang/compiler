module Ares.Compiler.Parser.Internal.StatementParser

open Ares.Compiler.Parser.Syntax.Statement
open Ares.Compiler.Parser.Internal.ParserUtils
open Ares.Compiler.Parser.Internal.TypeParamParser
open Ares.Compiler.Parser.Internal.ExpressionParser

open FParsec

let private sep = ';'
let private sepP = skipChar sep

module public VarDeclParser =
    let inferredTypeSpecificationParser = skipString "var" .>> spaces |>> fun _ -> Inferred
    let specificTypeSpecificationParser = typeDescriptorParser .>> spaces |>> fun td -> TypeDescriptor(td)
    let typeSpecificationParser = attempt inferredTypeSpecificationParser <|> specificTypeSpecificationParser
    let variableIdentifierParser = spaces >>. simpleIdentifierParser .>> spaces
    let assignedValueParser = spaces >>. skipChar '=' >>. spaces >>. expressionParser
    
    let varMapper = fun id ts av -> VariableDeclaration(id, ts, av)
    let public varDeclarationParser =
        (getGpsSpan (
            (pipe3
            typeSpecificationParser
            variableIdentifierParser
            assignedValueParser
            varMapper) .>> sepP)) |>> StatementSyntaxElement

module private DestructuringVarDeclParser =
    let destructuredDeclKeywordParser = skipString "var" .>> spaces
    let assignmentSeparator = spaces >>. skipChar '=' .>> spaces
    let identifierSeparator = skipChar ',' .>> spaces
    let objStartP = spaces >>. skipString "[[" >>. spaces
    let objEndP = spaces >>. skipString "]]" >>. spaces
    let declElementP = spaces >>. identifierParser .>> spaces
    let declarationBody = sepBy declElementP identifierSeparator
    let declaredVarsParser =
        destructuredDeclKeywordParser >>.
        objStartP >>. sepBy declElementP identifierSeparator .>> objEndP
    let assignedValueParser = assignmentSeparator >>. expressionParser
    let public destructuredDeclarationStmtParser =
        getGpsSpan (
            declaredVarsParser .>>. (assignedValueParser .>> sepP) |>> DestructuringVariableDeclaration)
        |>> StatementSyntaxElement

module private TypeDeclParser =
    let typeP = skipString "type" .>> spaces
    let typeParamSeperatorChar = ','
    let genericTypeDeclMapper = fun firstTp restTp -> firstTp :: restTp
    let typeParamListParser = sepBy1 typeParamParser (spaces >>. skipChar ',' .>> spaces)
    let typeAssignmentEqualP = spaces .>> skipChar '=' .>> spaces
    let nonGenericTypeDeclP =
        typeP >>. simpleTypeIdentifierParser .>> typeAssignmentEqualP .>>. typeDescriptorParser |>>
                             fun (id, td) -> TypeDeclaration(id, [], td)

    let genericTypeDeclP =
        pipe3
            (typeP >>. simpleTypeIdentifierParser)
            TypeDescriptorParsers.TypeParamParsers.typeParamsParser
            (skipChar '=' >>. spaces >>. typeDescriptorParser)
            (fun id tps td -> TypeDeclaration(id, tps, td))
    let public typeDeclarationStmtParser = getGpsSpan (choice [ attempt genericTypeDeclP; nonGenericTypeDeclP; ] .>> sepP) |>> StatementSyntaxElement

let returnStatementParser = getGpsSpan (
    (skipString "return" >>. skipChar ' ' >>. spaces >>. expressionParser .>> sepP) |>> Return) |>> StatementSyntaxElement

let private nonBlockStatementParser = choice [
    attempt returnStatementParser
    attempt TypeDeclParser.typeDeclarationStmtParser
    attempt DestructuringVarDeclParser.destructuredDeclarationStmtParser
    VarDeclParser.varDeclarationParser
]

let public statementParser = getGpsSpan (many (spaces >>. nonBlockStatementParser .>> spaces)) |>> fun (gps, s) ->
    if s.Length = 1 then s.Head
    else StatementSyntaxElement(gps, StmtStx.Block(s))