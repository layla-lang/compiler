module Ares.Compiler.Parser.Tests.Members.TypeDeclTests

open System
open Ares.Compiler.Parser.Syntax
open Ares.Compiler.Parser.Syntax.Expression
open Ares.Compiler.Parser.Syntax.Member
open Ares.Compiler.Parser.Syntax.TypeConstraint
open Ares.Compiler.Parser.Tests.Members.Common
open Xunit
open FsUnit.Xunit

[<Fact>]
let ``Can parse non-generic type declarations`` () =
    let exp = TypeDeclaration(
        Public,
        "X",
        [],
        !~Identified(!~Simple("Int"), []))
    parseMember "public type X = Int;" |> should equal exp

[<Fact>]
let ``Can parse generic func type declarations`` () =
    let exp = TypeDeclaration(
        Private,
        "X",
        [ !~{ Identifier = "'T"; Level = 1; } ],
        !~Func([], [], !~TypeArgument("'T")))
    parseMember "private type X<'T> = () => 'T;" |> should equal exp

[<Fact>]
let ``Can parse generic func type declaration with type arg in parameter`` () =
    let exp = TypeDeclaration(
        Protected,
        "X",
        [ !~{ Identifier = "'T"; Level = 1; } ],
        !~Func([], [!~TypeArgument("'T")], !~TypeArgument("'T")))
    parseMember "protected type X<'T> = ('T) => 'T;" |> should equal exp

[<Fact>]
let ``Can parse generic func type declaration with multiple parameters`` () =
    let exp = TypeDeclaration(
        Internal,
        "X",
        [ !~{ Identifier = "'T"; Level = 1; } ],
        !~Func([], [!~TypeArgument("'T"); !~TypeArgument("'T") ], !~TypeArgument("'T")))
    Assert.Equal(exp, parseMember "internal type X<'T> = ('T, 'T) => 'T;")

[<Fact>]
let ``Can parse generic type declarations assigned to record`` () =
    let exp = TypeDeclaration(
        Public,
        "ResultOf",
        [ !~{ Identifier = "'T"; Level = 1; } ],
        !~TypeDescriptor.Record(
            [
                { Identifier = !~Simple("Result"); Type = !~TypeArgument("'T"); }
            ]))
    Assert.Equal(
        exp,
        parseMember "public type ResultOf<'T> = { 'T Result; };")

[<Fact>]
let ``Can parse generic type declarations assigned to func returning tuple`` () =
    let exp = TypeDeclaration(
        Private,
        "ArrayDoubler",
        [ !~{ Identifier = "'T"; Level = 1; } ],
        !~Func(
            [],
            [ !~TypeDescriptor.TypeArgument("'T") ],
            !~TypeDescriptor.Tuple([ !~TypeDescriptor.TypeArgument("'T"); !~TypeDescriptor.TypeArgument("'T") ])))
    Assert.Equal(exp, parseMember "private type ArrayDoubler<'T> = ('T) => [['T, 'T]];")

[<Fact>]
let ``Can parse type declarations with single type param`` () =
    let tp: TypeParameter = { Identifier = "'T"; Level = 1; }
    Assert.Equal(
        TypeDeclaration(
            Protected,
            "WithNumber",
            [!~tp],
            !~TypeDescriptor.Record(
                [
                    { Identifier = !~Simple("number"); Type = !~TypeArgument("'T") }
                ]
            )), parseMember "protected type WithNumber<'T> = { 'T number; };")
    
[<Fact>]
let ``Can parse type declarations with multiple type params`` () =
    Assert.Equal(
        TypeDeclaration(
            Internal,
            "WithNumber",
            [
                !~{ Identifier = "'T"; Level = 1; }
                !~{ Identifier = "'U"; Level = 1; }
            ],
            !~TypeDescriptor.Record([
                    { Identifier = !~Simple("number"); Type = !~TypeArgument("'T") }
                ])
            ), parseMember "internal type WithNumber<'T, 'U> = { 'T number; };")

[<Fact>]
let ``Can parse indexed type declaration statements`` () =
    Assert.Equal(TypeDeclaration(
        Private,
        "Ind",
        [],
        !~TypeDescriptor.Indexed(
            !~Identified(!~Simple("Rec"), []),
            !~Literal( !~Value.StringLiteral("item") ))), parseMember "private type Ind = Rec[\"item\"];")
   
[<Fact>] 
let ``Can parse generic indexed type statements`` () =
    let result = parseMember "public type NameType = R<Int>[\"x\"];"
    Assert.Equal(TypeDeclaration(
        Public,
        "NameType",
        [],
        !~TypeDescriptor.Indexed(
            !~Identified(!~Simple("R"), [ !~Identified(!~Simple("Int"), []); ]),
            !~Literal( !~Value.StringLiteral("x") ))), result)