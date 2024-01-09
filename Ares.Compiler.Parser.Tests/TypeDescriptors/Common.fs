module private Ares.Compiler.Parser.Tests.TypeDescriptors.Common

open FParsec
open Ares.Compiler.Parser.State
open Ares.Compiler.Parser.Tests.TestUtils
open Ares.Compiler.Parser.CodeParser
open Ares.Compiler.Parser.Internal.ExpressionParser

let public parseTd str =
    let result = runParserOnString typeDescriptorParser ParserState.Default "" str
    match result with
        | Success (exp, _, _) ->
            eraseGps exp
            exp.TypeDescriptor
        | Failure (msg, err, _) -> raise (InternalParserException(msg, err))
