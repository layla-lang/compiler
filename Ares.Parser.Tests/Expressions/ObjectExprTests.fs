module Ares.Compiler.Parser.Tests.Expressions.ObjectExprTests

open Ares.Compiler.Parser.Tests.Expressions.Common
open Ares.Compiler.Parser.Syntax.Expression
open Ares.Compiler.Parser.Syntax.Value
open Xunit

[<Fact>]
let ``Can parse object expressions`` () =
        Assert.Equal(Expression.Object(Map([
                (!~Simple("hi"),  !~Constant(!~IntLiteral(4)))
        ])),
        parseExpression "{ hi: 4; }")
