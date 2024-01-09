module private Ares.Compiler.Parser.Tests.Members.Common

open FParsec
open Ares.Compiler.Parser.State
open Ares.Compiler.Parser.Tests.TestUtils
open Ares.Compiler.Parser.CodeParser
open Ares.Compiler.Parser.Internal.MemberParser

let public parseMember str =
    let result = runParserOnString memberParser ParserState.Default "" str
    match result with
        | Success (mem, _, _) ->
            eraseGps mem
            mem.Member
        | Failure (msg, err, _) -> raise (InternalParserException(msg, err))