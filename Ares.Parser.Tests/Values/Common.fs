module private Ares.Compiler.Parser.Tests.Values.Common

open FParsec
open Ares.Compiler.Parser.State
open Ares.Compiler.Parser.Tests.TestUtils
open Ares.Compiler.Parser.CodeParser
open Ares.Compiler.Parser.Internal.ValueParser

let public parseValue str =
    let result = runParserOnString valueParser ParserState.Default "" str
    match result with
        | Success (stmt, _, _) ->
            eraseGps stmt
            stmt.Literal
        | Failure (msg, err, _) -> raise (InternalParserException(msg, err))