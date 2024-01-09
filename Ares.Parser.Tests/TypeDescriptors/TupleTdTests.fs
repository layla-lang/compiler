module Ares.Compiler.Parser.Tests.TypeDescriptors.TupleTdTests

open Ares.Compiler.Parser.Tests.TypeDescriptors.Common
open Ares.Compiler.Parser.Syntax.Expression
open FsUnit.Xunit
open Xunit

[<Fact>]
let ``Can parse tuple type descriptors`` () =
    Assert.Equal(Tuple([
        !~Identified(!~Simple("Int"), [])
        !~Identified(!~Simple("String"), [])
    ]), parseTd "[[Int, String]]")
