module Ares.Compiler.Parser.Tests.Expressions.ArrayExprTests

open Ares.Compiler.Parser.Tests.Expressions.Common
open Ares.Compiler.Parser.Syntax.Expression
open Ares.Compiler.Parser.Syntax.Value
open Xunit

[<Fact>]
let ``Can parse array expressions with single elements`` () =
        Assert.Equal(Expression.Array([
            !~Constant(!~IntLiteral(4))
        ]),
        parseExpression "[4]")
        
[<Fact>]
let ``Can parse array expressions with multiple elements`` () =
        Assert.Equal(Expression.Array([
            !~Constant(!~IntLiteral(4))
            !~Constant(!~IntLiteral(3))
        ]),
        parseExpression "[4, 3]")

