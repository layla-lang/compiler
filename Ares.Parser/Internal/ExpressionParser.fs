module Ares.Compiler.Parser.Internal.ExpressionParser

open Ares.Compiler.Parser.State
open Ares.Compiler.Parser.Syntax
open Ares.Compiler.Parser.Syntax.Common
open Ares.Compiler.Parser.Syntax.Expression
open Ares.Compiler.Parser.Internal.CommonParsers
open Ares.Compiler.Parser.Internal.ParserUtils
open Ares.Compiler.Parser.Internal.ValueParser

open FParsec

let typeDescriptorParser, tdParserRef = createParserForwardedToRef<TypeDescriptorSyntaxElement, ParserState>()

let simpleIdentifierParser = getGpsSpan (simpleIdStringParser |>> Simple) |>> IdentifierSyntaxElement
let simpleTypeIdentifierParser = getGpsSpan (simpleUppercaseIdStringParser |>> Simple) |>> IdentifierSyntaxElement

let identifierParser, private identifierParserRef = createParserForwardedToRef<IdentifierSyntaxElement, ParserState>()
let typeIdentifierParser = simpleUppercaseIdStringParser
let expressionParser, private expressionParserRef = createParserForwardedToRef<ExpressionSyntaxElement, ParserState>()

module private IdParsers =

    let accessorChar = '.'
    let memberOfCombinerMapper = fun f rest -> f :: rest

    let indexAccessMapper = fun id expr -> IndexAccess(id, expr)
    let indexAccessIdentifier = getGpsSpan (pipe2 simpleIdentifierParser (skipChar '[' >>. expressionParser .>> skipChar ']') indexAccessMapper) |>> IdentifierSyntaxElement
    let nonMemberIdentifiers = choice [
        simpleIdentifierParser
        indexAccessIdentifier
    ]
    let memberOfMapper = fun first rest ->
        let combined = first :: rest
        MemberAccess(combined.GetSlice(Some(0), Some(combined.Length - 2)), combined.Item(combined.Length - 1))
    let memberOfParser = getGpsSpan (pipe2
                                          (nonMemberIdentifiers .>> skipChar accessorChar)
                                          (sepBy1 nonMemberIdentifiers (skipChar accessorChar))
                                          memberOfMapper) |>> IdentifierSyntaxElement
                        

module public TypeDescriptorParsers =
    
    let neverTypeDescriptorParser = getGpsSpan (skipString "never" >>. preturn Never) |>> TypeDescriptorSyntaxElement
    
    module TypeArgumentParsers =
        let public typeArgumentTdParser =
            getGpsSpan (typeArgumentIdParser |>> TypeArgument) |>> TypeDescriptorSyntaxElement
        let public typeArgsParser =
            createCharDelimitedExpressionParser '<' '>' ','  typeDescriptorParser

    module TypeParamParsers =
        let public typeParamsParser =
            let tpSyntaxParser = getGpsSpan typeParameterParser |>> TypeParameterSyntaxElement
            createCharDelimitedExpressionParser '<' '>' ','  tpSyntaxParser
    
    module private IdentifiedTdParsers =
        let nonGenericP = getGpsSpan (
            simpleTypeIdentifierParser .>> spaces |>>
            fun id -> Identified(id, [])) |>> TypeDescriptorSyntaxElement

        let genericP =
            getGpsSpan (
                pipe2
                    (simpleTypeIdentifierParser .>> spaces)
                    (TypeArgumentParsers.typeArgsParser)
                    (fun id args -> Identified(id, args))) |>> TypeDescriptorSyntaxElement
        let idTypeDescriptorParser = attempt genericP <|> nonGenericP
        
    module private TupleTdParsers =
        let public tupleTdParser =
            getGpsSpan (createStrDelimitedExpressionParser "[[" "]]" ',' typeDescriptorParser |>> Tuple) |>> TypeDescriptorSyntaxElement

    module private FuncTdParsers =
        let paramsParser = createCharDelimitedExpressionParser '(' ')' ',' typeDescriptorParser
        
        let arrowNotationP = spaces >>. skipString "=>" >>. spaces
        let returnTypeParser = arrowNotationP >>. typeDescriptorParser
        let nonGenericParser =
            pipe2
                paramsParser
                returnTypeParser
                (fun ps result -> Func([], ps, result))
                
        let genericParser =
            pipe3
                TypeParamParsers.typeParamsParser
                paramsParser
                returnTypeParser
                (fun targs ps result -> Func(targs, ps, result))
        let public funcTdParser = getGpsSpan (nonGenericParser <|> genericParser) |>> TypeDescriptorSyntaxElement

    module private RecordTdParsers =
        let memberTerminatorP = skipChar ';' .>> spaces
        let recordStartP = spaces >>. skipChar '{' >>. spaces
        let recordEndP = spaces >>. skipChar '}' >>. spaces
        let memberTypeParser = spaces >>. typeDescriptorParser .>> spaces
        let memberNameParser = simpleIdentifierParser .>> memberTerminatorP
        let memberMapper = fun t id -> { Identifier = id; Type = t; }
        let memberP = pipe2 memberTypeParser memberNameParser memberMapper
        let memberBodyP = many memberP
        let recordTypeDescriptorParser = getGpsSpan (recordStartP >>. memberBodyP .>> recordEndP) |>> fun (gps, members) -> TypeDescriptorSyntaxElement(gps, Record(members))

    module private LiteralTdParsers =
        let numberTypeLiteralParser = getGpsSpan ((numberLiteralParser .>> spaces) |>> Literal) |>> TypeDescriptorSyntaxElement
        let stringTypeLiteralParser = getGpsSpan ((stringParser .>> spaces) |>> Literal) |>> TypeDescriptorSyntaxElement
        let boolTypeLiteralParser = getGpsSpan ((boolParser .>> spaces) |>> Literal) |>> TypeDescriptorSyntaxElement

        let public typeLiteralParser: Parser<TypeDescriptorSyntaxElement, ParserState> = choice [
            numberTypeLiteralParser
            stringTypeLiteralParser
            boolTypeLiteralParser
        ]
    
    module private IndexedTdParsers =
        let openB = skipChar '['
        let closeB = skipChar ']'
        let indexableParsers = choice [
            IdentifiedTdParsers.idTypeDescriptorParser
            attempt RecordTdParsers.recordTypeDescriptorParser
            attempt TupleTdParsers.tupleTdParser
        ]
        let indexedTypeP = indexableParsers
        let indexerP = openB >>. typeDescriptorParser .>> closeB
        let indexedTdParser = getGpsSpan (indexedTypeP .>>. indexerP |>> Indexed) |>> TypeDescriptorSyntaxElement
    
    module TdOperatorParsers =
        let public tdOperatorParser = OperatorPrecedenceParser<TypeDescriptorSyntaxElement, _, ParserState>()
        let expr: Parser<TypeDescriptorSyntaxElement, ParserState> = tdOperatorParser.ExpressionParser
        let typeTerm = choice [
            neverTypeDescriptorParser
            attempt TypeArgumentParsers.typeArgumentTdParser
            attempt IndexedTdParsers.indexedTdParser
            attempt LiteralTdParsers.typeLiteralParser
            attempt RecordTdParsers.recordTypeDescriptorParser
            attempt FuncTdParsers.funcTdParser
            attempt TupleTdParsers.tupleTdParser
            IdentifiedTdParsers.idTypeDescriptorParser
        ]
        let pword s = pstring s .>> spaces
        let parens p = between (pword "(") (pword ")") p
        let typeTermParser = choice [
            typeTerm .>> spaces
            parens expr
        ]

        let ws = spaces

        type Assoc = Associativity

        let adjustPosition offset (pos: Position) =
            Position(pos.StreamName, pos.Index + int64 offset,
                     pos.Line, pos.Column + int64 offset)

        let addInfixOperator str prec assoc mapping =
            let op = InfixOperator(str, getPosition .>> ws, prec, assoc, (),
                                   fun opPos leftTerm rightTerm ->
                                       let adjustedStart = toGps (adjustPosition -str.Length opPos)
                                       let adjustedEnd = toGps (opPos)
                                       let cs: GpsSpan = { Start = adjustedStart; End = adjustedEnd; }
                                       mapping cs leftTerm rightTerm)
            tdOperatorParser.AddOperator(op)

        let addPostfixOperator str prec assoc mapping =
            let op = PostfixOperator(str, getPosition .>> ws, prec, assoc, (),
                                   fun opPos t ->
                                       let adjustedStart = toGps (adjustPosition -str.Length opPos)
                                       let adjustedEnd = toGps (opPos)
                                       let cs: GpsSpan = { Start = adjustedStart; End = adjustedEnd; }
                                       mapping cs t)
            tdOperatorParser.AddOperator(op)

        addInfixOperator "|" 1 Associativity.Left (fun gps x y -> TypeDescriptorSyntaxElement(gps, Union([x; y])))
        addInfixOperator "&" 2 Associativity.Left (fun gps x y -> TypeDescriptorSyntaxElement(gps, Intersection([x; y])))
        addPostfixOperator "[]" 3 false (fun gps x -> TypeDescriptorSyntaxElement(gps, Array(x)))
        tdOperatorParser.TermParser <- typeTermParser

open TypeDescriptorParsers

module internal ExprParsers =
    
    let pword s = pstring s .>> spaces
    let parens p = between (pword "(") (pword ")") p

    let numberExpressionParser = getGpsSpan ((numberLiteralParser .>> spaces) |>> Constant) |>> ExpressionSyntaxElement
    let stringExpressionParser = getGpsSpan ((stringParser .>> spaces) |>> Constant) |>> ExpressionSyntaxElement
    let boolExpressionParser = getGpsSpan ((boolParser .>> spaces) |>> Constant) |>> ExpressionSyntaxElement
    let variableParser = getGpsSpan ((identifierParser .>> spaces) |>> Variable) |>> ExpressionSyntaxElement

    module internal LambdaExprParsers =
        let lambdaParamStartP = spaces >>. skipChar '(' >>. spaces
        let lambdaParamEndP = spaces >>. skipChar ')' >>. spaces
        let lambdaParamsPMapper = fun p1 pRest -> p1 :: pRest
        let inferredParamParser = identifierParser |>> fun id ->
            let p: LambdaParameter = { Identifier = id; Type = LambdaParameterType.Inferred }
            p
        let typeSpecifiedParamParser =
            pipe2
                (typeDescriptorParser .>> spaces)
                identifierParser
                (fun td id ->
                    let p: LambdaParameter = { Identifier = id; Type = TypeDescriptor(td) }
                    p)
        let lambdaParamParser = typeSpecifiedParamParser <|> inferredParamParser
        let lambdaParamsP = sepBy lambdaParamParser (skipChar ',' .>> spaces)
        let dblArrowNotationP = spaces >>. skipString "=>" >>. spaces
        let lambdaExpressionParser: Parser<ExpressionSyntaxElement, ParserState> =
                getGpsSpan (
                    pipe2
                        (lambdaParamStartP >>. lambdaParamsP .>> lambdaParamEndP .>> dblArrowNotationP)
                        (expressionParser)
                        (fun ps result -> Lambda(ps, result))) |>> ExpressionSyntaxElement

    module internal ObjectExprParsers =
        let assignmentSeparator = skipChar ':' .>> spaces
        let memberSeparator = skipChar ';' .>> spaces
        let objStartP = spaces >>. skipChar '{' >>. spaces
        let objEndP = spaces >>. skipChar '}' >>. spaces
        let memberNameParser = spaces >>. simpleIdentifierParser .>> assignmentSeparator
        let memberValueParser = spaces >>. expressionParser .>> spaces .>> memberSeparator
        let memberMapper = fun id t -> (id, t)
        let memberP = pipe2 memberNameParser memberValueParser memberMapper
        let memberBodyP = many memberP |>> fun members -> Map(members)
        let objectExpressionParser = getGpsSpan (objStartP >>. memberBodyP .>> objEndP |>> Object) |>> ExpressionSyntaxElement
   
    module internal TupleExprParsers =
        let tupleP = createStrDelimitedExpressionParser "[[" "]]" ',' expressionParser
        let public tupleExpressionParser =
            getGpsSpan (tupleP |>> Expression.Tuple)
            |>> ExpressionSyntaxElement

    module internal ArrayExprParsers =
        let arrayP = createCharDelimitedExpressionParser '[' ']' ',' expressionParser
        let public arrayExpressionParser =
            getGpsSpan (arrayP |>> Expression.Array)
            |>> ExpressionSyntaxElement
     
    module internal InvocationExprParsers =
        let invStartP = skipChar '(' >>. spaces
        let invEndP = spaces >>. skipChar ')' >>. spaces
        let paramSeparatorP = skipChar ',' .>> spaces
        let invParamsP = sepBy expressionParser paramSeparatorP
        let invocationExpP =
            pipe3
                (identifierParser)
                (attempt TypeArgumentParsers.typeArgsParser <|> preturn [])
                (invStartP >>. invParamsP .>> invEndP)
                (fun id targs pars -> Invocation(id, targs, pars))
        let invocationExpressionParser =
            getGpsSpan invocationExpP |>> ExpressionSyntaxElement
    
    module internal CastExprParsers =
        let parens p = between (pword "(") (pword ")") p
        let castExprParser =
            getGpsSpan(pipe2
                (parens typeDescriptorParser)
                (expressionParser)
                (fun td exp -> Cast(td, exp))) |>> ExpressionSyntaxElement

    module internal IsTypeExprParser =
        let isNotInIsTypeExpr =
            userStateSatisfies (fun us -> us.InIsTypeExpr = false)
        let enterIsTypeExpr: Parser<unit, ParserState> =
            updateUserState (fun us -> {us with InIsTypeExpr = true})
        let exitIsTypeExpr: Parser<unit, ParserState> =
            updateUserState (fun us -> {us with InIsTypeExpr = false})

        let isTypeP =
            isNotInIsTypeExpr >>.
            enterIsTypeExpr >>.
            expressionParser .>> spaces .>>
            skipString "is" .>> spaces .>>.
            typeDescriptorParser
            .>> exitIsTypeExpr
            |>> IsType
        let isTypeExprParser =
            getGpsSpan isTypeP |>> ExpressionSyntaxElement
    
    let private constParser: Parser<ExpressionSyntaxElement, ParserState> = choice [
        numberExpressionParser
        stringExpressionParser
        boolExpressionParser
        variableParser
    ]
    
    let public operationParser = OperatorPrecedenceParser<ExpressionSyntaxElement, _, ParserState>()
    let expr: Parser<ExpressionSyntaxElement, _> = operationParser.ExpressionParser

    let private term: Parser<ExpressionSyntaxElement, ParserState> = choice [
        attempt IsTypeExprParser.isTypeExprParser
        attempt CastExprParsers.castExprParser
        attempt TupleExprParsers.tupleExpressionParser
        attempt ArrayExprParsers.arrayExpressionParser
        attempt InvocationExprParsers.invocationExpressionParser
        attempt ObjectExprParsers.objectExpressionParser
        constParser
        attempt LambdaExprParsers.lambdaExpressionParser
        parens expr
    ]

    let ws = spaces
    let posWS = getPosition .>> ws

    type Assoc = Associativity
    let adjustPosition offset (pos: Position) =
        Position(pos.StreamName, pos.Index + int64 offset,
                 pos.Line, pos.Column + int64 offset)

    // To simplify infix operator definitions, we define a helper function.
    let addInfixOperator str prec assoc mapping =
        let op = InfixOperator(str, posWS, prec, assoc, (),
                               fun opPos leftTerm rightTerm ->
                                   let adjustedStart = toGps (adjustPosition -str.Length opPos)
                                   let adjustedEnd = toGps opPos
                                   let sp: GpsSpan = { Start = adjustedStart; End = adjustedEnd; }
                                   mapping sp leftTerm rightTerm)
        operationParser.AddOperator(op)
    
    let addTernaryOperator lc rc prec assoc mapping =
        let top = TernaryOperator(
            lc, posWS, rc, posWS,
            prec, assoc, (), fun leftPos rightPos leftTerm rightTerm mt ->
                                   let adjustedStartLeft = toGps (adjustPosition -lc.Length leftPos)
                                   let adjustedEndRight = toGps rightPos
                                   let cs: GpsSpan = { Start = adjustedStartLeft; End = adjustedEndRight; }
                                   mapping cs leftTerm rightTerm mt)
        operationParser.AddOperator(top)
        
    let private createOperation op = fun codeSpan left right -> ExpressionSyntaxElement(codeSpan, Operation (left, op, right))
    addTernaryOperator "?" ":" 1 Associativity.Left (fun cs predicate ifTrue ifFalse -> ExpressionSyntaxElement(cs, Ternary (predicate, ifTrue, ifFalse)))
    addInfixOperator "^"  2 Associativity.Left (createOperation Operator.Power)
    addInfixOperator "/"  3  Associativity.Left (createOperation Operator.Division)
    addInfixOperator "*"  4  Associativity.Left (createOperation Operator.Multiplication)
    addInfixOperator "%"  5 Associativity.Left (createOperation Operator.Modulus)
    addInfixOperator "-"  6  Associativity.Left (createOperation Operator.Subtraction)
    addInfixOperator "+"  7  Associativity.Left (createOperation Operator.Addition)
    addInfixOperator ">"  8  Associativity.Left (createOperation Operator.Gt)
    addInfixOperator ">=" 9  Associativity.Left (createOperation Operator.Gte)
    addInfixOperator "<"  10  Associativity.Left (createOperation Operator.Lt)
    addInfixOperator "<=" 11 Associativity.Left (createOperation Operator.Lte)
    addInfixOperator "==" 12 Associativity.Left (createOperation Operator.Equals)
    addInfixOperator "!=" 13 Associativity.Left (createOperation Operator.NotEquals)
    addInfixOperator "&&" 14 Associativity.Left (createOperation Operator.BoolAnd)
    addInfixOperator "||" 15 Associativity.Left (createOperation Operator.BoolOr)
    operationParser.TermParser <- term

open ExprParsers

do expressionParserRef := ExprParsers.operationParser
do identifierParserRef := choice [
    attempt IdParsers.memberOfParser
    attempt IdParsers.indexAccessIdentifier
    simpleIdentifierParser
]
do tdParserRef := TypeDescriptorParsers.TdOperatorParsers.tdOperatorParser


let numberTypeLiteralParser = getGpsSpan ((numberLiteralParser .>> spaces) |>> Literal) |>> TypeDescriptorSyntaxElement
let stringTypeLiteralParser = getGpsSpan ((stringParser .>> spaces) |>> Literal) |>> TypeDescriptorSyntaxElement
let boolTypeLiteralParser = getGpsSpan ((boolParser .>> spaces) |>> Literal) |>> TypeDescriptorSyntaxElement

let public typeLiteralParser: Parser<TypeDescriptorSyntaxElement, ParserState> = choice [
    numberTypeLiteralParser
    stringTypeLiteralParser
    boolTypeLiteralParser
]