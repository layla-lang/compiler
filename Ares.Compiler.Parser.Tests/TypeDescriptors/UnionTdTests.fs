module Ares.Compiler.Parser.Tests.TypeDescriptors.UnionTdTests

open Ares.Compiler.Parser.Tests.TypeDescriptors.Common
open Ares.Compiler.Parser.Syntax.Expression
open Ares.Compiler.Parser.Syntax.Value
open FsUnit.Xunit
open Xunit

[<Fact>]
let ``Can parse type union descriptors`` () =
    (parseTd "Int | String") |> should equal (Union(
        [!~Identified(!~Simple("Int"), [])
         !~Identified(!~Simple("String"), [])]))
    
[<Fact>]
let ``Can parse union of complex descriptors`` () =
    parseTd "IntResult[\"Result\"] | String" |> should equal (Union(
        [
         !~Indexed(
            !~Identified(!~Simple("IntResult"), []),
            !~Literal(!~StringLiteral("Result")))
         !~Identified(!~Simple("String"), [])
        ]))
    // TODO: flatten unions and intersections of more than two items
    (* Assert.Equal(Union(
        [Id(Simple("hello"))
         Id(Simple("hi"))
         Id(Simple("how_are_you"))]), parseTd "hello | hi | how_are_you") *)