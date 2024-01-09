module Ares.Compiler.Parser.Internal.TypeConstraintParser

open Ares.Compiler.Parser.Syntax.TypeConstraint
open Ares.Compiler.Parser.Internal.ExpressionParser
open Ares.Compiler.Parser.Internal.ParserUtils

open FParsec

let operators = [
    "+"; "-"; "*"; "/"; "%"; "^"; // Math
    ">"; ">="; "<"; "<="; "=="; "!=" // Comparison
]

let makeListParser eleParser =
    let singleElementP = eleParser
    let sep = skipChar ',' >>. spaces
    let multipleElementP = many (sep >>. eleParser)
    pipe2
        singleElementP
        multipleElementP
        (fun se ml -> se :: ml)

module private IsClosedUnderTcParser =
    let operatorParser = operators |> List.map pstringCI |> choice
    let isClosedUnderParser =
        skipString "is closed under" >>. skipChar ' ' .>> spaces >>. makeListParser operatorParser |>> IsClosedUnder
    let public isClosedUnderTcParser = getGpsSpan isClosedUnderParser |>> TypeConstraintSyntaxElement

module private ExtendsTcParser =
    let implementsParser =
        skipString "extends" >>. skipChar ' ' .>> spaces >>. makeListParser typeDescriptorParser |>> Extends
    let public extendsTcParser = getGpsSpan implementsParser |>> TypeConstraintSyntaxElement


let typeConstraintParser = choice [
    IsClosedUnderTcParser.isClosedUnderTcParser
    ExtendsTcParser.extendsTcParser
]