module Ares.Compiler.Parser.Tests.Statements.ReturnStmtTests

open Ares.Compiler.Parser.CodeParser
open Ares.Compiler.Parser.Internal
open Ares.Compiler.Parser.Syntax
open Ares.Compiler.Parser.Syntax.Expression
open Ares.Compiler.Parser.Syntax.Value
open Ares.Compiler.Parser.Syntax.Statement
open Ares.Compiler.Parser.Tests.Statements.Common
open Xunit

[<Fact>]
let ``Can parse return statements`` () =
    Assert.Equal(Return(!~Constant(!~IntLiteral(4))), parseStatement "return 4;")
