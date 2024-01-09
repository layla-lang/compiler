module Ares.Compiler.Parser.Tests.TypeParams.GeneralTypeParamTests

open Ares.Compiler.Parser.Tests.TypeParams.Common
open Ares.Compiler.Parser.CodeParser
open Xunit
open FsUnit.Xunit

[<Fact>]
let ``Does not parse type parameters beginning with lowercase letters`` () = 
    (fun () -> parseTp "blah" |> ignore) |> should throw typeof<InternalParserException>

[<Fact>]
let ``Does not parse type parameters not beginning in single quote`` () = 
    (fun () -> parseTp "T" |> ignore) |> should throw typeof<InternalParserException>