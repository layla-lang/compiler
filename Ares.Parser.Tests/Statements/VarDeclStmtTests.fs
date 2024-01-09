module Ares.Compiler.Parser.Tests.Statements.VarDeclStmtTests

open Ares.Compiler.Parser.CodeParser
open Ares.Compiler.Parser.Internal
open Ares.Compiler.Parser.Syntax.Expression
open Ares.Compiler.Parser.Syntax.Value
open Ares.Compiler.Parser.Syntax.Statement
open Ares.Compiler.Parser.Internal.StatementParser
open Ares.Compiler.Parser.Tests.TestUtils
open Ares.Compiler.Parser.Tests.Statements.Common
open Xunit
open FsUnit.Xunit
open FParsec

[<Fact>]
let ``Can parse variable declaration statement with inferred type`` () =
    Assert.Equal(VariableDeclaration(
        Inferred, !~Simple("x"), !~Operation(
            !~Constant(!~IntLiteral(4)),
            Operator.Addition,
            !~Constant(!~IntLiteral(2)))), parseStatement "var x = 4 + 2;")

[<Fact>]
let ``Can parse variable declaration statement with explicit type`` () =
    Assert.Equal(VariableDeclaration(
        TypeDescriptor(
            !~Identified(!~Simple("Int"), [])),
        !~Simple("x"),
        !~Operation(
            !~Constant(!~IntLiteral(4)),
            Operator.Addition,
            !~Constant(!~IntLiteral(2)))), parseStatement "Int x = 4 + 2;")
    
[<Fact>]
let ``Can parse variable declaration statement assigning to Lambda inferred type`` () =
    let l = !~Lambda([
            { Identifier = !~Simple("p1"); Type = LambdaParameterType.TypeDescriptor(!~Identified(!~Simple("Int"), [])) }
        ], 
        !~Operation(
            !~Variable(!~Simple("p1")),
            Operator.Multiplication,
            !~Variable(!~Simple("p1"))))

    Assert.Equal(VariableDeclaration(Inferred, !~Simple("squarer"), l), parseStatement "var squarer = (Int p1) => p1 * p1;")
