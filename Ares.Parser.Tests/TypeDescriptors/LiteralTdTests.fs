module Ares.Compiler.Parser.Tests.TypeDescriptors.LiteralTdTests

open Ares.Compiler.Parser.Tests.TypeDescriptors.Common
open Ares.Compiler.Parser.Syntax.Expression
open Ares.Compiler.Parser.Syntax.Value
open FsUnit.Xunit
open Xunit

[<Fact>]
let ``Can parse literal types`` () =
    parseTd "\"Hi\"" |> should equal (Literal(!~StringLiteral("Hi")))
    parseTd "4" |> should equal (Literal(!~IntLiteral(4)))
    parseTd "3.14" |> should equal (Literal(!~FloatLiteral(3.14)))
    parseTd "true" |> should equal (Literal(!~BoolLiteral(true)))
    parseTd "false" |> should equal (Literal(!~BoolLiteral(false)))
