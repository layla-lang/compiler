module Ares.Compiler.Parser.Tests.Expressions.InvocationExprTests

open Ares.Compiler.Parser.Tests.Expressions.Common
open Ares.Compiler.Parser.Syntax.Expression
open Ares.Compiler.Parser.Syntax.Value
open Xunit

[<Fact>]
let ``Can parse parameterless invocation expression`` () =
    Assert.Equal(Invocation(!~Simple("Method"), [], []), parseExpression("Method()"))

[<Fact>]
let ``Can parse invocation expression with a single parameter`` () =
    Assert.Equal(Invocation(
        !~Simple("Square"), [],
        [
            !~Constant(!~IntLiteral(3))
        ]), parseExpression("Square(3)"))


[<Fact>]
let ``Can parse invocation expression with parameters`` () =
    Assert.Equal(Invocation(
        !~Simple("Method"), [],
        [
            !~Constant(!~IntLiteral(2))
            !~Constant(!~FloatLiteral(3.14))
        ]), parseExpression("Method(2, 3.14)"))

[<Fact>]
let ``Can parse nested invocation expression`` () =
    Assert.Equal(Invocation(
        !~Simple("Method1"), [],
        [
            !~Invocation(
                !~Simple("Method2"), [],
                [
                  !~Constant(!~IntLiteral(2))
                ])
            !~Constant(!~FloatLiteral(3.14))
        ]), parseExpression("Method1(Method2(2), 3.14)"))

[<Fact>]
let ``Can parse invocation expression after member access identifier`` () =
    Assert.Equal(Invocation(
        !~MemberAccess([
          !~Simple("x")
          !~Simple("y")
        ], !~Simple("Method")), [],
        [
            !~Constant(!~IntLiteral(2))
            !~Constant(!~FloatLiteral(3.14))
        ]), parseExpression("x.y.Method(2, 3.14)"))

[<Fact>]
let ``Can parse invocation expression with nested expressions as parameters`` () =
    Assert.Equal(Invocation(
        !~Simple("Method"), [],
        [
            !~Operation(
                !~Invocation(
                    !~MemberAccess([ !~Simple("x"); !~Simple("y") ], !~Simple("AnotherMethod")), [],
                    [ !~Constant(!~StringLiteral("hi")) ]),
                Operator.Addition,
                !~Variable(!~Simple("x")))
            !~Constant(!~IntLiteral(2))
        ]), parseExpression("Method(x.y.AnotherMethod(\"hi\") + x, 2)"))

[<Fact>]
let ``Can parse invocation expression with type arguments`` () =
    Assert.Equal(Invocation(
        !~Simple("Method"),
        [
            !~Identified(!~Simple("Int"), [])
        ],
        [
            !~Constant(!~IntLiteral(2))
            !~Constant(!~FloatLiteral(3.14))
        ]), parseExpression("Method<Int>(2, 3.14)"))
