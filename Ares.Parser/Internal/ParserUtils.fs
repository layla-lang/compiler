namespace Ares.Compiler.Parser.Internal

open Ares.Compiler.Parser.State
open Ares.Compiler.Parser.Syntax.Common
open FParsec

module ParserUtils =
  let toGps (pos: Position) = {
    Index = pos.Index
    Line = pos.Line
    Column = pos.Column
    Location = pos.StreamName
  }
  let getGps: Parser<SourceGps, ParserState> = getPosition |>> fun p -> toGps p
  let getGpsSpan p = (getGps .>>. (p .>>. getGps)) |>> fun pars ->
    let (st, (parsed, en)) = pars
    let cs: GpsSpan = {
      Start = st
      End = en 
    }
    cs, parsed