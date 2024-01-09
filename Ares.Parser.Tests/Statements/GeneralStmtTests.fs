module Ares.Compiler.Parser.Tests.Statements.GeneralStmtTests

open Ares.Compiler.Parser.CodeParser
open Ares.Compiler.Parser.Tests.Statements.Common
open Ares.Compiler.Parser.Syntax.Statement
open Xunit
open FsUnit.Xunit

[<Fact>]
let ``Requires statement end in semicolon`` () =
    (fun () -> parseStatement "var x = 4 + 2" |> ignore) |> should throw typeof<System.Exception>
    (fun () -> parseStatement "var x = 4 + 2" |> ignore) |> should throw typeof<System.Exception>
    (fun () -> parseStatement "var x = 4 + 2" |> ignore) |> should throw typeof<System.Exception>

[<Fact>]
let ``Statements ending in semicolon correctly parsed`` () =
    (parseStatement "x = 4 + 2;")      |> should be instanceOfType<StmtStx>
    (parseStatement "x: Int = 4 + 2;") |> should be instanceOfType<StmtStx>
    (parseStatement "type X = Int;")   |> should be instanceOfType<StmtStx>