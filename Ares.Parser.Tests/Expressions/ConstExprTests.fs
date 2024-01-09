module Ares.Compiler.Parser.Tests.Expressions.ConstExprTests

open Ares.Compiler.Parser.Tests.Expressions.Common
open Ares.Compiler.Parser.Syntax.Value
open Xunit

[<Fact>]
let ``Can parse constant expressions`` () =
    let intExpr = parseExpression "4"
    let floatExpr = parseExpression "3.14"
    let boolTrueExpr = parseExpression "true"
    let boolFalseExpr = parseExpression "false"
    let strExpr = parseExpression "\"Hello World!\""
    Assert.Equal(IntLiteral(4), literalOf intExpr)
    Assert.Equal(FloatLiteral(3.14), literalOf floatExpr)
    Assert.Equal(BoolLiteral(true), literalOf boolTrueExpr)
    Assert.Equal(BoolLiteral(false), literalOf boolFalseExpr)
    Assert.Equal(StringLiteral("Hello World!"), literalOf strExpr)