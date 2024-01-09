module Ares.Compiler.Parser.Tests.Statements.TypeDeclStmtTests

open Ares.Compiler.Parser.Syntax
open Ares.Compiler.Parser.Syntax.Expression
open Ares.Compiler.Parser.Syntax.Statement

open Ares.Compiler.Parser.Tests.Statements.Common
open Xunit
open FsUnit.Xunit

[<Fact>]
let ``Can parse non-generic type declarations`` () =
    let exp = TypeDeclaration(
        !~Simple("X"),
        [],
        !~Identified(!~Simple("Int"), []))
    parseStatement "type X = Int;" |> should equal exp

[<Fact>]
let ``Can parse generic func type declarations`` () =
    let exp = TypeDeclaration(
        !~Simple("X"),
        [ !~{ Identifier = "'T"; Level = 1; } ],
        !~Func([], [], !~TypeArgument("'T")))
    parseStatement "type X<'T> = () => 'T;" |> should equal exp

[<Fact>]
let ``Can parse generic func type declaration with type arg in parameter`` () =
    let exp = TypeDeclaration(
        !~Simple("X"),
        [ !~{ Identifier = "'T"; Level = 1; } ],
        !~Func([], [!~TypeArgument("'T")], !~TypeArgument("'T")))
    parseStatement "type X<'T> = ('T) => 'T;" |> should equal exp

[<Fact>]
let ``Can parse generic func type declaration with multiple parameters`` () =
    let exp = TypeDeclaration(
        !~Simple("X"),
        [ !~{ Identifier = "'T"; Level = 1; } ],
        !~Func([], [!~TypeArgument("'T"); !~TypeArgument("'T") ], !~TypeArgument("'T")))
    parseStatement "type X<'T> = ('T, 'T) => 'T;" |> should equal exp

[<Fact>]
let ``Can parse generic type declarations assigned to record`` () =
    let exp = TypeDeclaration(
        !~Simple("ResultOf"),
        [ !~{ Identifier = "'T"; Level = 1; } ],
        !~TypeDescriptor.Record(
            [
                { Identifier = !~Simple("Result"); Type = !~TypeArgument("'T"); }
            ]))
    Assert.Equal(
        exp,
        parseStatement "type ResultOf<'T> = { 'T Result; };")

[<Fact>]
let ``Can parse generic type declarations assigned to func returning tuple`` () =
    let exp = TypeDeclaration(
        !~Simple("ArrayDoubler"),
        [ !~{ Identifier = "'T"; Level = 1; } ],
        !~Func(
            [],
            [ !~TypeDescriptor.TypeArgument("'T") ],
            !~TypeDescriptor.Tuple([ !~TypeDescriptor.TypeArgument("'T"); !~TypeDescriptor.TypeArgument("'T") ])))
    Assert.Equal(exp, parseStatement "type ArrayDoubler<'T> = ('T) => [['T, 'T]];")

[<Fact>]
let ``Can parse type declarations with single type param`` () =
    let tp: TypeParameter = { Identifier = "'T"; Level = 1; }
    Assert.Equal(
        TypeDeclaration(
            !~Simple("WithNumber"),
            [!~tp],
            !~TypeDescriptor.Record(
                [
                    { Identifier = !~Simple("number"); Type = !~TypeArgument("'T") }
                ]
            )), parseStatement "type WithNumber<'T> = { 'T number; };")
    
[<Fact>]
let ``Can parse type declarations with multiple type params`` () =
    Assert.Equal(
        TypeDeclaration(
            !~Simple("WithNumber"),
            [
                !~{ Identifier = "'T"; Level = 1; }
                !~{ Identifier = "'U"; Level = 1; }
            ],
            !~TypeDescriptor.Record([
                    { Identifier = !~Simple("number"); Type = !~TypeArgument("'T") }
                ])
            ), parseStatement "type WithNumber<'T, 'U> = { 'T number; };")

[<Fact>]
let ``Can parse indexed type declaration statements`` () =
    Assert.Equal(TypeDeclaration(
        !~Simple("Ind"),
        [],
        !~TypeDescriptor.Indexed(
            !~Identified(!~Simple("Rec"), []),
            !~Literal( !~Value.StringLiteral("item") ))), parseStatement "type Ind = Rec[\"item\"];")
   
[<Fact>] 
let ``Can parse generic indexed type statements`` () =
    let result = parseStatement "type NameType = R<Int>[\"x\"];"
    Assert.Equal(TypeDeclaration(
        !~Simple("NameType"),
        [],
        !~TypeDescriptor.Indexed(
            !~Identified(!~Simple("R"), [ !~Identified(!~Simple("Int"), []); ]),
            !~Literal( !~Value.StringLiteral("x") ))), result)