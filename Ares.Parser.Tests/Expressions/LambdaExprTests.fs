module Ares.Compiler.Parser.Tests.Expressions.LambdaExprTests

open Ares.Compiler.Parser.Tests.Expressions.Common
open Ares.Compiler.Parser.Syntax.Expression
open Xunit

[<Fact>]
let ``Can parse Lambda function with inferred types`` () =
        Assert.Equal(Lambda([
                { Identifier = !~Simple("x"); Type = Inferred }
                { Identifier = !~Simple("y"); Type = Inferred }
            ],
            !~Expression.Operation(
                !~Expression.Variable(!~Simple("x")),
                Operator.Addition,
                !~Expression.Variable(!~Simple("y")))),
        parseExpression "(x, y) => x + y")

[<Fact>]
let ``Can parse Lambda function with specified types`` () =
        Assert.Equal(Lambda([
                { Identifier = !~Simple("x"); Type = TypeDescriptor(!~Identified(!~Simple("Int"), [])) }
                { Identifier = !~Simple("y"); Type = TypeDescriptor(!~Identified(!~Simple("String"), [])) }
            ],
            !~Expression.Operation(
                !~Expression.Variable(!~Simple("x")),
                Operator.Addition,
                !~Expression.Variable(!~Simple("y")))),
        parseExpression "(Int x, String y) => x + y")