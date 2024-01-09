module private Ares.Compiler.Parser.Tests.Statements.Common

open FParsec
open Ares.Compiler.Parser.State
open Ares.Compiler.Parser.Tests.TestUtils
open Ares.Compiler.Parser.CodeParser
open Ares.Compiler.Parser.Internal.StatementParser

let public parseStatement str =
    let result = runParserOnString statementParser ParserState.Default "" str
    match result with
        | Success (stmt, _, _) ->
            eraseGps stmt
            stmt.Statement
        | Failure (msg, err, _) -> raise (InternalParserException(msg, err))