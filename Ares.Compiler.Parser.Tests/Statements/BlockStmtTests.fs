module Ares.Compiler.Parser.Tests.Statements.BlockStmtTests

open System
open Ares.Compiler.Parser.CodeParser
open Ares.Compiler.Parser.Syntax.Expression
open Ares.Compiler.Parser.Syntax.Value
open Ares.Compiler.Parser.Syntax.Statement
open Ares.Compiler.Parser.Tests.Statements.Common
open Xunit

[<Fact>]
let ``Can parse block statements on multiple lines`` () =
    let stmt = parseStatement """
var x = 4;
Int y = 2 + 3;
"""
    let d1 = VariableDeclaration(
            Inferred,
            !~Simple("x"),
            !~Constant(!~IntLiteral(4)))
    let d2 = VariableDeclaration(
            TypeDescriptor(!~Identified(!~Simple("Int"), [])),
            !~Simple("y"),
            !~Operation(
                !~Constant(!~IntLiteral(2)),
                Operator.Addition,
                !~Constant(!~IntLiteral(3))))
    Assert.Equal(Block([ !~d1; !~d2; ]), stmt)

[<Fact>]
let ``Single statement blocks are flattened`` () =

    let stmt = parseStatement """
var x = 4;
"""
    let decl = VariableDeclaration(
            Inferred,
            !~Simple("x"),
            !~Constant(!~IntLiteral(4)))
    Assert.Equal(decl, stmt)
    
let indexedBlockStatements = """
type R<'T> = {
  'T x;
};
type NameType = R<Int>["x"];
"""
    
[<Fact>]    
let ``Parses indexed statements`` () =
    let stmts = match parseStatement (indexedBlockStatements.Trim()) with
                | Block (stmts) -> stmts
                | _ -> raise (Exception("Wah"))
    
    Assert.Equal(2, stmts.Length)