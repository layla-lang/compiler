module Ares.Compiler.Parser.Tests.Identifiers.IndexAccessTests

open Ares.Compiler.Parser.Tests.Identifiers.Common
open Ares.Compiler.Parser.Syntax.Expression
open Ares.Compiler.Parser.Syntax.Value
open Xunit

[<Fact>]
let ``Can parse index access identifiers`` () =
    Assert.Equal(IndexAccess(!~Simple("hello"), !~Constant(!~StringLiteral("hi"))), parseIdentifier "hello[\"hi\"]")
    Assert.Equal(IndexAccess(!~Simple("hello"), !~Constant(!~IntLiteral(4))), parseIdentifier "hello[4]")
    Assert.Equal(IndexAccess(!~Simple("hello"), !~Variable(!~MemberAccess([!~Simple("hi")], !~Simple("how_are_you")))), parseIdentifier "hello[hi.how_are_you]")