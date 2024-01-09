namespace Ares.Compiler.Parser

open Ares.Compiler.Parser.State
open FParsec
open Ares.Compiler.Parser.Internal

module CodeParser =
    
    exception public InternalParserException of ErrorMessage: string * error: ParserError
    
    let parseElement eleParser sourceStr =
        let result = runParserOnString eleParser ParserState.Default "code" sourceStr
        match result with
        | Success(res, _, _) -> res
        | Failure(msg, err, _) -> raise (InternalParserException(msg, err))
        
    let public parseContext sourceStr = parseElement ContextParser.contextParser sourceStr
    let public parseStatement sourceStr = parseElement StatementParser.statementParser sourceStr
    let public parseExpression sourceStr = parseElement ExpressionParser.expressionParser sourceStr
    let public parseIdentifier sourceStr = parseElement ExpressionParser.identifierParser sourceStr
    let public parseTypeDescriptor sourceStr = parseElement ExpressionParser.typeDescriptorParser sourceStr
    let public parseTypeParameter sourceStr = parseElement TypeParamParser.typeParamParser sourceStr
    let public parseValue sourceStr = parseElement ValueParser.valueParser sourceStr