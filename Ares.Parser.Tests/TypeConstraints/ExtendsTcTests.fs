module Ares.Compiler.Parser.Tests.TypeConstraints.ExtendsTcTests

open Ares.Compiler.Parser.Syntax.Expression
open Ares.Compiler.Parser.Syntax.TypeConstraint
open Ares.Compiler.Parser.Tests.TypeConstraints.Common
open Xunit

[<Fact>]
let ``Can parse single type 'extends' constraint`` () =
    Assert.Equal(Extends([
        !~Identified(!~Simple("X"), [])
    ]), parseTc "extends X")

[<Fact>]
let ``Can parse multiple types with generic type args in a 'extends' constraint`` () =
    Assert.Equal(Extends([
        !~Identified(!~Simple("X"), [])
        !~Identified(!~Simple("X"), [
            !~Identified(!~Simple("String"), [])
        ])
    ]), parseTc "extends X, X<String>")