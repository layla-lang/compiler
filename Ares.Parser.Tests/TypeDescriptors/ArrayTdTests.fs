module Ares.Compiler.Parser.Tests.TypeDescriptors.ArrayTdTests

open Ares.Compiler.Parser.Tests.TypeDescriptors.Common
open Ares.Compiler.Parser.Syntax.Expression
open FsUnit.Xunit
open Xunit

[<Fact>]
let ``Can parse simple array descriptors`` () =
    Assert.Equal(Array(!~Identified(!~Simple("String"), [])), parseTd "String[]")

[<Fact>]
let ``Can parse array descriptors of more complex types`` () =
    Assert.Equal(Array(!~Union([
        !~Identified(!~Simple("Int"), [])
        !~Identified(!~Simple("String"), [])
    ])), parseTd "(Int | String)[]")