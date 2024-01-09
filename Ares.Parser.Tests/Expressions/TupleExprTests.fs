module Ares.Compiler.Parser.Tests.Expressions.TupleExprTests

open Ares.Compiler.Parser.Tests.Expressions.Common
open Ares.Compiler.Parser.Syntax.Expression
open Ares.Compiler.Parser.Syntax.Value
open Xunit

[<Fact>]
let ``Can parse tuple expressions`` () =
        Assert.Equal(Expression.Tuple([
            !~Constant(!~IntLiteral(4))
            !~Constant(!~IntLiteral(3))
        ]),
        parseExpression "[[4, 3]]")

