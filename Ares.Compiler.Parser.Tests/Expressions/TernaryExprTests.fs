module Ares.Compiler.Parser.Tests.Expressions.TernaryExprTests

open Ares.Compiler.Parser.Tests.Expressions.Common
open Ares.Compiler.Parser.Syntax.Expression
open Xunit

[<Fact>]
let ``Can parse ternary expressions`` () =
        Assert.Equal(Expression.Ternary(
                !~Expression.Variable(!~Simple("x")),
                !~Expression.Variable(!~Simple("y")),
                !~Expression.Variable(!~Simple("z"))),
        parseExpression "x ? y : z")
