module Ares.Compiler.Parser.Tests.TypeDescriptors.FuncTdTests

open Ares.Compiler.Parser.Tests.TypeDescriptors.Common
open Ares.Compiler.Parser.Syntax.Expression
open Xunit


[<Fact>]
let ``Can parse funcs with no arguments`` () =
    Assert.Equal(Func([], [], !~Identified(!~Simple("Bool"), [])), parseTd "() => Bool")

[<Fact>]
let ``Can parse func non-generic type descriptors`` () =
    Assert.Equal(Func([], [
        !~Identified(!~Simple("Int"), [])
        !~Identified(!~Simple("String"), [])
    ], !~Identified(!~Simple("Bool"), [])), parseTd "(Int, String) => Bool")


[<Fact>]
let ``Can parse generic type descriptors`` () =
    let tp: TypeParameter = { Identifier = "'T"; Level = 1; }
    Assert.Equal(
        Func([ !~tp ], [
        !~Identified(!~Simple("Int"), [])
        !~Identified(!~Simple("String"), [])
    ], !~TypeArgument("'T")), parseTd "<'T>(Int, String) => 'T")