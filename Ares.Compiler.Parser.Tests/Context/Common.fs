module private Ares.Compiler.Parser.Tests.Context.Common

open FParsec
open Ares.Compiler.Parser.State
open Ares.Compiler.Parser.Tests.TestUtils
open Ares.Compiler.Parser.CodeParser
open Ares.Compiler.Parser.Internal.ContextParser

let public parseContext str =
    let result = runParserOnString contextParser ParserState.Default "" str
    match result with
        | Success (exp, _, _) ->
            eraseGps exp
            exp
        | Failure (msg, err, _) ->
            raise (InternalParserException(msg, err))