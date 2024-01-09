module Ares.Compiler.Parser.Tests.Expressions.OperationExprTests

open Ares.Compiler.Parser.Tests.Expressions.Common
open Ares.Compiler.Parser.Syntax.Expression
open Ares.Compiler.Parser.Syntax.Value
open Xunit

[<Fact>]
let ``Can parse simple binary operation expressions`` () =
    Assert.Equal(
        Expression.Operation(
            !~Expression.Constant(!~Literal.IntLiteral(4)),
            Operator.Division,
            !~Expression.Constant(!~Literal.IntLiteral(3))),
        parseExpression "4 / 3")
    Assert.Equal(
        Expression.Operation(
            !~Expression.Constant(!~Literal.FloatLiteral(3.14)),
            Operator.Multiplication,
            !~Expression.Constant(!~Literal.IntLiteral(1))),
        parseExpression "3.14 * 1")
    Assert.Equal(
        Expression.Operation(
            !~Expression.Constant(!~Literal.BoolLiteral(true)),
            Operator.Subtraction,
            !~Expression.Constant(!~Literal.BoolLiteral(false))),
        parseExpression "true - false")
    Assert.Equal(
        Expression.Operation(
            !~Expression.Constant(!~Literal.StringLiteral("hi")),
            Operator.Addition,
            !~Expression.Constant(!~Literal.StringLiteral("h"))),
        parseExpression "\"hi\" + \"h\"")
    Assert.Equal(
        Expression.Operation(
            !~Expression.Constant(!~Literal.IntLiteral(2)),
            Operator.Power,
            !~Expression.Constant(!~Literal.IntLiteral(3))),
        parseExpression "2 ^ 3")
    Assert.Equal(
        Expression.Operation(
            !~Expression.Variable(!~Simple("z")),
            Operator.Modulus,
            !~Expression.Variable(!~Simple("q"))),
        parseExpression "z % q")

// TODO: Verify this
[<Fact>]
let ``Operation expressions obey order of operations`` () =
        Assert.Equal(
            Expression.Operation(
                !~Expression.Operation(
                    !~Expression.Constant(!~IntLiteral(2)),
                    Operator.Multiplication,
                    !~Expression.Operation(
                        !~Expression.Constant(!~IntLiteral(5)),
                        Operator.Subtraction,
                        !~Expression.Constant(!~IntLiteral(4))
                    )
                ),
                Operator.Division,
                !~Expression.Variable(!~Simple("q"))),
        parseExpression "2 * 5 - 4 / q")