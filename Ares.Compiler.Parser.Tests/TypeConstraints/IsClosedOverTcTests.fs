module Ares.Compiler.Parser.Tests.TypeConstraints.IsClosedOverTcTests

open Ares.Compiler.Parser.CodeParser
open Ares.Compiler.Parser.Syntax.TypeConstraint
open Ares.Compiler.Parser.Tests.TypeConstraints.Common
open Xunit
open FsUnit.Xunit

[<Fact>]
let ``Can parse single operator 'is closed under' constraint`` () =
    Assert.Equal(IsClosedUnder(["+"]), parseTc "is closed under +")


[<Fact>]
let ``Can parse multi operator 'is closed under' constraint`` () =
    Assert.Equal(IsClosedUnder(["+"; "-"; "*"]), parseTc "is closed under +, -, *")

[<Fact>]
let ``Cannot parse unknown operator for 'is closed over' constraint`` () =
    (fun () -> parseTc "is closed over ~" |> ignore) |> should throw typeof<InternalParserException>


