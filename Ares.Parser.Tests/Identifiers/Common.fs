module private Ares.Compiler.Parser.Tests.Identifiers.Common

open FParsec
open Ares.Compiler.Parser.State
open Ares.Compiler.Parser.Tests.TestUtils
open Ares.Compiler.Parser.CodeParser
open Ares.Compiler.Parser.Internal.ExpressionParser

let public parseIdentifier str =
    let result = runParserOnString identifierParser ParserState.Default "" str
    match result with
        | Success (exp, _, _) ->
            eraseGps exp
            exp.Identifier
        | Failure (msg, err, _) -> raise (InternalParserException(msg, err))