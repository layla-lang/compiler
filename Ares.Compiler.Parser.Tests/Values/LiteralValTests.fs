module Ares.Compiler.Parser.Tests.Values.LiteralValTests

open Ares.Compiler.Parser.Tests.Values.Common
open Ares.Compiler.Parser.Syntax.Value
open Xunit

[<Fact>]
let ``Can parse constant expressions`` () =
    Assert.Equal(IntLiteral(4), parseValue "4")
    Assert.Equal(FloatLiteral(3.14), parseValue "3.14")
    Assert.Equal(BoolLiteral(true), parseValue "true")
    Assert.Equal(BoolLiteral(false), parseValue "false")
    Assert.Equal(StringLiteral("Hello World!"), parseValue "\"Hello World!\"")