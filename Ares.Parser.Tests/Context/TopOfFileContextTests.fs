module Ares.Compiler.Parser.Tests.Context.TopOfFileContextTests

open Ares.Compiler.Parser.CodeParser
open Ares.Compiler.Parser.Tests.Context.Common
open Xunit
open FsUnit.Xunit

let contextSampleAtBeginning =
    """context Hi;
       
       public type X = String;
    """
let contextWithMultipleMembers =
    """context Hi;
       
       public type X = String;
       public type Y = Int;
    """
let contextSampleNotAtBeginning = """

context Hi;
       
type X = String;
"""

[<Fact>]
let ``Can parse context at beginning`` () =
    Assert.Equal("Hi", (parseContext contextSampleAtBeginning).Name)

[<Fact>]
let ``Can parse context with multiple members`` () =
    Assert.Equal(2, (parseContext contextWithMultipleMembers).Body.Length)

[<Fact>]
let ``Cannot parse context that is not at beginning of file`` () =
    (fun () -> parseContext contextSampleNotAtBeginning |> ignore) |> should throw typeof<InternalParserException>


