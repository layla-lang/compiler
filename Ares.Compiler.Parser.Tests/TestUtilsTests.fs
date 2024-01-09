module Ares.Compiler.Parser.Tests.TestUtilsTests

open Ares.Compiler.Parser.State
open Ares.Compiler.Parser.CodeParser
open Ares.Compiler.Parser.Syntax.Common
open Xunit
open FParsec
open Ares.Compiler.Parser.Syntax.Value
open Ares.Compiler.Parser.Internal.ValueParser
open Ares.Compiler.Parser.Internal.ExpressionParser
open Ares.Compiler.Parser.Tests.TestUtils

let parseExpression str =
    let result = runParserOnString expressionParser ParserState.Default "" str
    match result with
        | Success (exp, _, _) -> exp.Expression
        | Failure (msg, err, _) -> raise (InternalParserException(msg, err))

[<Fact>]
let ``SourceGps equality works correctly`` () =
    let geo1 = {
        Location = "hi"
        Index = 123
        Line = 456
        Column = 789 
    }
    let geo2 = {
        Location = "hello"
        Index = 123
        Line = 456
        Column = 789 
    }
    Assert.NotEqual(geo1, geo2)
    let geo3 = {
        Location = "hi"
        Index = 123
        Line = 456
        Column = 789 
    }
    Assert.Equal(geo1, geo3)

[<Fact>]
let ``SyntaxElement correctly calculates equality`` () =
    let geo1 = {
        Location = "hi.ares"
        Index = 1
        Line = 2
        Column = 3 
    }
    let geo2 = {
        Location = "hi.ares"
        Index = 1
        Line = 2
        Column = 4 
    }
    let geo3 = {
        Location = "hi.ares"
        Index = 1
        Line = 2
        Column = 4 
    }
    let geo4 = {
        Location = "hi.ares"
        Index = 100
        Line = 2
        Column = 4 
    }

    Assert.NotEqual(
        ValueSyntaxElement({ Start = geo1; End = geo4; }, IntLiteral(4)),
        ValueSyntaxElement({ Start = geo1; End = geo4; }, IntLiteral(5)))
    Assert.Equal(
        ValueSyntaxElement({ Start = geo1; End = geo4; }, IntLiteral(4)),
        ValueSyntaxElement({ Start = geo1; End = geo4; }, IntLiteral(4)))
    Assert.NotEqual(
        ValueSyntaxElement({ Start = geo1; End = geo4; }, IntLiteral(5)),
        ValueSyntaxElement({ Start = geo2; End = geo4; }, IntLiteral(5)))
    Assert.Equal(
        ValueSyntaxElement({ Start = geo2; End = geo4; }, IntLiteral(5)),
        ValueSyntaxElement({ Start = geo3; End = geo4; }, IntLiteral(5)))
    
[<Fact>]
let ``Can parse constant expressions`` () =
    let aVal = !~IntLiteral(4)
    let pVal: ValueSyntaxElement = parseValue "4"
    Assert.NotEqual(aVal.CodeSpan, pVal.CodeSpan);
    Assert.NotEqual(aVal, pVal)
    eraseGps pVal
    Assert.Equal(aVal.CodeSpan, pVal.CodeSpan);
    Assert.Equal(aVal, pVal)
    