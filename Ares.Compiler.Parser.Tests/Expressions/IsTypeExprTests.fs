module Ares.Compiler.Parser.Tests.Expressions.IsTypeExprTests

open Ares.Compiler.Parser.Tests.Expressions.Common
open Ares.Compiler.Parser.Syntax.Expression
open Xunit

[<Fact>]
let ``Can parse is type expression`` () =
    Assert.Equal(IsType(
        !~Variable(!~Simple("x")),
        !~Identified(!~Simple("Int"), [])), parseExpression("x is Int"))
