module Ares.Compiler.Parser.Tests.Expressions.CastExprTests

open Ares.Compiler.Parser.Tests.Expressions.Common
open Ares.Compiler.Parser.Syntax.Expression
open Ares.Compiler.Parser.Syntax.Value
open Xunit

[<Fact>]
let ``Can parse cast expressions`` () =
        Assert.Equal(Expression.Cast(
                !~Identified(!~Simple("Int"), []),
                !~Expression.Constant(!~IntLiteral(3))),
        parseExpression "(Int)3")
