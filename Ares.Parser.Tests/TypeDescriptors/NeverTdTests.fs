module Ares.Compiler.Parser.Tests.TypeDescriptors.NeverTdTests

open Ares.Compiler.Parser.Tests.TypeDescriptors.Common
open Ares.Compiler.Parser.Syntax.Expression
open Xunit

[<Fact>]
let ``Can parse never type`` () =
    Assert.Equal(Never, parseTd "never")

