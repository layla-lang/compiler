module Ares.Compiler.Parser.Internal.TypeParamParser

open Ares.Compiler.Parser.Syntax.Expression
open Ares.Compiler.Parser.Internal.CommonParsers
open Ares.Compiler.Parser.Internal.ParserUtils
open FParsec

let typeParamParser = getGpsSpan (typeParameterParser) |>> fun (cs, p) -> TypeParameterSyntaxElement(cs, p)