module Ares.Compiler.Parser.Tests.TypeConstraints.Common

open FParsec
open Ares.Compiler.Parser.State
open Ares.Compiler.Parser.Tests.TestUtils
open Ares.Compiler.Parser.CodeParser
open Ares.Compiler.Parser.Internal.TypeConstraintParser

let public parseTc str =
    let result = runParserOnString typeConstraintParser ParserState.Default "" str
    match result with
        | Success (tc, _, _) ->
            eraseGps tc
            tc.TypeConstraint
        | Failure (msg, err, _) -> raise (InternalParserException(msg, err))
