module Ares.Compiler.Parser.Tests.Statements.DestructuredVarDeclStmtTests

open Ares.Compiler.Parser.CodeParser
open Ares.Compiler.Parser.Internal
open Ares.Compiler.Parser.Syntax
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
let ``Can parse destructured var declaration statements`` () =
    Assert.Equal(DestructuringVariableDeclaration([
        !~Simple("x")
        !~Simple("y")
        !~Simple("z")
    ], !~Variable(!~Simple("tup3"))), parseStatement "var [[x, y, z]] = tup3;")
