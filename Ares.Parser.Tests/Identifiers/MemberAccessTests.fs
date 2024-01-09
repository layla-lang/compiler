module Ares.Compiler.Parser.Tests.Identifiers.MemberAccessTests

open Ares.Compiler.Parser.Tests.Identifiers.Common
open Ares.Compiler.Parser.Syntax.Expression
open Xunit

[<Fact>]
let ``Can parse member access identifiers`` () =
    Assert.Equal(MemberAccess([!~Simple("hello")], !~Simple("hi")), parseIdentifier "hello.hi")
    