module Ares.Compiler.Parser.Tests.Identifiers.SimpleIdTests

open Ares.Compiler.Parser.Tests.Identifiers.Common
open Ares.Compiler.Parser.Syntax.Expression
open Xunit


[<Fact>]
let ``Can parse simple identifiers`` () =
    Assert.Equal(Simple("hello"), parseIdentifier "hello")
    Assert.Equal(Simple("_9"), parseIdentifier "_9")
    Assert.Equal(Simple("how_are_you"), parseIdentifier "how_are_you")