module Ares.Compiler.Parser.Tests.TypeParams.NoConstraintTpTests

open Ares.Compiler.Parser.Tests.TypeParams.Common
open Ares.Compiler.Parser.Syntax.Expression
open Xunit

[<Fact>]
let ``Can parse type parameters without constraints`` () =
    Assert.Equal({ Identifier = "'U"; Level = 1; }, parseTp "'U")
    Assert.Equal({ Identifier = "'Element"; Level = 1; }, parseTp "'Element")

