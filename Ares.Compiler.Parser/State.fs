module Ares.Compiler.Parser.State

type ParserState =
    { InIsTypeExpr: bool }
    with
       static member Default = {InIsTypeExpr = false;}
