module Ares.Compiler.Parser.Internal.ContextParser

open Ares.Compiler.Parser.Internal.CommonParsers
open Ares.Compiler.Parser.Internal.ParserUtils
open Ares.Compiler.Parser.Internal.MemberParser
open Ares.Compiler.Parser.Syntax.Context
open FParsec

let atBeginning: Parser<'a,'u> =
    fun stream ->
        let posReply = getPosition stream
        if posReply.Result.Line > 1 then
            let error = messageError "Context statements must only appear at the beginning of the file."
            Reply(Error, error)
        else posReply

let conKeywordP = skipString "context" >>. spaces
let conSemicolon = spaces .>> skipChar ';'
let bodyParser = manyTill (spaces >>. memberParser .>> spaces) eof
let conP = pipe2
               (conKeywordP >>. simpleUppercaseIdStringParser .>> conSemicolon)
               (bodyParser)
               (fun name body -> (name, body))
let contextParser =
    (getGpsSpan (spaces >>. atBeginning >>. conP)) |>>
        fun (gps, con) ->
            let (name, body) = con
            ContextSyntaxElement(gps, name, body)