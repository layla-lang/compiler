module private Ares.Compiler.Parser.Tests.Expressions.Common

open System
open Ares.Compiler.Parser.State
open FParsec
open Ares.Compiler.Parser.Tests.TestUtils
open Ares.Compiler.Parser.CodeParser
open Ares.Compiler.Parser.Syntax.Expression
open Ares.Compiler.Parser.Internal.ExpressionParser

let public parseExpression str =
    let result = runParserOnString expressionParser ParserState.Default "" str
    match result with
        | Success (exp, _, _) ->
            eraseGps exp
            exp.Expression
        | Failure (msg, err, _) -> raise (InternalParserException(msg, err))

let literalOf expr =
    match expr with
    | Constant c -> c.Literal
    | _ -> raise(ArgumentException("Not a constant expression."))