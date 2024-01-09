module Ares.Compiler.Parser.Tests.TypeDescriptors.TypeRefTdTests

open Ares.Compiler.Parser.Tests.TypeDescriptors.Common
open Ares.Compiler.Parser.Syntax.Expression
open Xunit

[<Fact>]
let ``Can parse non-generic type reference descriptors`` () =
    Assert.Equal(Identified(!~Simple("Int"), []), parseTd "Int")
    Assert.Equal(Identified(!~Simple("X"), []), parseTd "X")
    
[<Fact>]
let ``Can parse generic type reference descriptors`` () =
    Assert.Equal(Identified(
        !~Simple("X"),
        [
            !~Identified(!~Simple("Bool"), [])
        ]), parseTd "X<Bool>")
    Assert.Equal(Identified(
        !~Simple("Y"),
        [
            !~Identified(!~Simple("Int"), [])
            !~Identified(!~Simple("String"), [])
        ]), parseTd "Y<Int, String>")