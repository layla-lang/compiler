module Ares.Compiler.Parser.Tests.TypeDescriptors.RecordTdTests

open Ares.Compiler.Parser.Tests.TypeDescriptors.Common
open Ares.Compiler.Parser.Syntax.Expression
open FsUnit.Xunit

[<Xunit.Fact>]
let ``Can parse record descriptors`` () =
    let recordStr = """
{
  Int | Float birthday;
  Int[] ids;
  String name;
  {
    Int cvv;
    String number;
  } paymentMethod;
}
"""
    let nameM = { Identifier = !~Simple("name"); Type = !~Identified(!~Simple("String"), []) }
    let idsM = { Identifier = !~Simple("ids"); Type = !~Array(!~Identified(!~Simple("Int"), [])) }
    let birthdayM = {
                    Identifier = !~Simple("birthday")
                    Type = !~Union([
                    !~Identified(!~Simple("Int"), [])
                    !~Identified(!~Simple("Float"), [])
                ])}
    let paymentMethodM = { Identifier = !~Simple("paymentMethod"); Type = !~Record(
                    [
                        { Identifier = !~Simple("cvv"); Type =  !~Identified(!~Simple("Int"), []) }
                        { Identifier = !~Simple("number"); Type = !~Identified(!~Simple("String"), []) }
                    ]
                )}
    let exp = Record([
                birthdayM
                idsM
                nameM
                paymentMethodM
                ])
    (parseTd recordStr) |> should equal exp