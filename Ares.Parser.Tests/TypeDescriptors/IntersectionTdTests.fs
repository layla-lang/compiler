module Ares.Compiler.Parser.Tests.TypeDescriptors.IntersectionTdTests

open Ares.Compiler.Parser.Tests.TypeDescriptors.Common
open Ares.Compiler.Parser.Syntax.Expression
open FsUnit.Xunit
open Xunit

[<Fact>]
let ``Can parse type intersection descriptors`` () =
    Assert.Equal(Intersection(
        [!~Identified(!~Simple("Int"), [])
         !~Identified(!~Simple("String"), [])]), parseTd "Int & String")