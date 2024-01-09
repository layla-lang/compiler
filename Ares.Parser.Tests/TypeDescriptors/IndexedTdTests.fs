module Ares.Compiler.Parser.Tests.TypeDescriptors.IndexedTdTests

open Ares.Compiler.Parser.Tests.TypeDescriptors.Common
open Ares.Compiler.Parser.Syntax.Expression
open Ares.Compiler.Parser.Syntax.Value
open Xunit

[<Fact>]
let ``Can parse indexed types`` () =
    Assert.Equal(Indexed(
        !~Identified(!~Simple("Map"), []),
        !~Literal(!~StringLiteral("hi"))),
    parseTd "Map[\"hi\"]")

[<Fact>]
let ``Can indexed by union types`` () =
    Assert.Equal(Indexed(
        !~Identified(!~Simple("Map"), []),
        !~Union([
            !~Literal(!~StringLiteral("hi"))
            !~Literal(!~StringLiteral("hello"))
        ])), parseTd "Map[\"hi\"|\"hello\"]")