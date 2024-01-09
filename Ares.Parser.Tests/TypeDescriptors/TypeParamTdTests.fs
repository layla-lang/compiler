module Ares.Compiler.Parser.Tests.TypeDescriptors.TypeParamTdTests

open Ares.Compiler.Parser.Tests.TypeDescriptors.Common
open Ares.Compiler.Parser.Syntax.Expression
open Xunit

[<Fact>]
let ``Can parse type param type`` () =
    Assert.Equal(TypeArgument("'T"), parseTd "'T")

