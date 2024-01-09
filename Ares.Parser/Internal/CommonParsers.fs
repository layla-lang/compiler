module Ares.Compiler.Parser.Internal.CommonParsers

open System
open System.Globalization
open Ares.Compiler.Parser.State
open Ares.Compiler.Parser.Syntax.Expression
open FParsec
open Ares.Compiler.Parser.Internal.ParserUtils

let private sep = ';'

let _createDelimitedExpressionParser sp ep sepChar itemParser =
   
    let elementSeparator = skipChar sepChar .>> spaces
    let startP = spaces .>> sp >>. spaces
    let endP = spaces .>> ep >>. spaces
    let elementP = spaces >>. itemParser .>> spaces
    let singleElementP = elementP
    let multipleElementP = many (elementSeparator >>. elementP)
    let elementP =
            pipe2
                singleElementP
                multipleElementP
                (fun se ml -> se :: ml)
    
    let delimitedParser = startP >>. (elementP <|> preturn []) .>> endP
    delimitedParser
    
let createCharDelimitedExpressionParser startChar endChar sepChar itemParser =
    _createDelimitedExpressionParser (skipChar startChar) (skipChar endChar) sepChar itemParser

let createStrDelimitedExpressionParser startStr endStr sepChar itemParser =
    let startP: Parser<unit, _> = skipString startStr
    let endP: Parser<unit, _> = skipString endStr
    _createDelimitedExpressionParser startP endP sepChar itemParser

module private IdParsers =
    let validIdChar c =
        match Char.GetUnicodeCategory(c) with
        | UnicodeCategory.UppercaseLetter | UnicodeCategory.LowercaseLetter | UnicodeCategory.TitlecaseLetter | UnicodeCategory.OtherLetter | UnicodeCategory.LetterNumber -> true
        | UnicodeCategory.DecimalDigitNumber -> true
        | UnicodeCategory.ConnectorPunctuation -> true
        | UnicodeCategory.Format -> true
        | _ -> false
    let validUppercaseStartingIdChar c =
        match Char.GetUnicodeCategory(c) with
        | UnicodeCategory.UppercaseLetter -> true
        | _ -> false
    let validStartingIdChar c =
        match Char.GetUnicodeCategory(c) with
        | UnicodeCategory.UppercaseLetter | UnicodeCategory.LowercaseLetter | UnicodeCategory.TitlecaseLetter | UnicodeCategory.OtherLetter | UnicodeCategory.LetterNumber -> true
        | _ -> false
    let startingCharParser = pchar '_' <|> pchar '\u0005' <|> satisfy validStartingIdChar
    let idCombiner = fun x y -> x.ToString() + y
    let uppercaseCharParser = satisfy validUppercaseStartingIdChar

let simpleIdStringParser: Parser<string, ParserState> = pipe2 IdParsers.startingCharParser (manySatisfy IdParsers.validIdChar) IdParsers.idCombiner
let simpleUppercaseIdStringParser = pipe2 IdParsers.uppercaseCharParser (manySatisfy IdParsers.validIdChar) IdParsers.idCombiner

let typeParamTildePrefixParser = many1Chars (pchar '\'')

let typeParameterParser =
    (typeParamTildePrefixParser .>>. simpleUppercaseIdStringParser) |>>
    fun (tildes, id) -> { Identifier = tildes + id; Level = tildes.Length; }
let typeArgumentIdParser =
    (typeParamTildePrefixParser .>>. simpleUppercaseIdStringParser) |>> fun (t, s) -> t + s
    
let typeParamsParser =
    let tpSyntaxParser = getGpsSpan typeParameterParser |>> TypeParameterSyntaxElement
    createCharDelimitedExpressionParser '<' '>' ','  tpSyntaxParser
