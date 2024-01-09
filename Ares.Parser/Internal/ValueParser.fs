module Ares.Compiler.Parser.Internal.ValueParser

open Ares.Compiler.Parser.Syntax.Value
open Ares.Compiler.Parser.Internal.ParserUtils
open FParsec

let numberLiteralParser =
    getGpsSpan (numberLiteral (NumberLiteralOptions.DefaultInteger ||| NumberLiteralOptions.DefaultFloat) "number")
    |>> fun (gps, lit) ->
            if lit.IsInteger then ValueSyntaxElement(gps, IntLiteral(int lit.String))
            else ValueSyntaxElement(gps, FloatLiteral(float lit.String))
    
let trueP = getGpsSpan ((pstring "true" .>> spaces) |>> fun _ -> BoolLiteral(true)) |>> ValueSyntaxElement
let falseP = getGpsSpan ((pstring "false" .>> spaces) |>> fun _ -> BoolLiteral(false)) |>> ValueSyntaxElement
let boolParser = trueP <|> falseP

let dblQuote = skipChar '\"'
let stringParser = getGpsSpan (dblQuote >>. manyCharsTill anyChar dblQuote)
                |>> fun (gps, strLit) -> ValueSyntaxElement(gps, StringLiteral(strLit))
                .>> spaces
let public valueParser: Parser<ValueSyntaxElement, _> =
    choice [
        numberLiteralParser
        stringParser
        boolParser
    ] |>> fun f -> f