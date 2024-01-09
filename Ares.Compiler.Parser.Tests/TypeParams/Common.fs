module Ares.Compiler.Parser.Tests.TypeParams.Common

open FParsec
open Ares.Compiler.Parser.State
open Ares.Compiler.Parser.Tests.TestUtils
open Ares.Compiler.Parser.CodeParser
open Ares.Compiler.Parser.Internal.TypeParamParser

let parseTp str =
    let result = runParserOnString typeParamParser ParserState.Default "" str
    match result with
        | Success (exp, _, _) ->
            eraseGps exp
            exp.TypeParameter
        | Failure (msg, err, _) -> raise (InternalParserException(msg, err))