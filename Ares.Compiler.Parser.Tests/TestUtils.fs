module Ares.Compiler.Parser.Tests.TestUtils

open System
open Ares.Compiler.Parser.Syntax.Common
open Ares.Compiler.Parser.Syntax.Context
open Ares.Compiler.Parser.Syntax.Expression
open Ares.Compiler.Parser.Syntax.Member
open Ares.Compiler.Parser.Syntax.Statement
open Ares.Compiler.Parser.Syntax.Value
open Ares.Compiler.Parser.Syntax.TypeConstraint

let rec eraseGps (ele: SyntaxElement) =
    ele.EraseGpsForTesting()
    match ele with
    | :? ValueSyntaxElement as valSyntax -> ()
    | :? IdentifierSyntaxElement as idSyntax ->
        match idSyntax.Identifier with
        | Simple s -> ()
        | MemberAccess (path, id) ->
            for c in path do eraseGps c
            eraseGps id
        | IndexAccess (id, expr) ->
            eraseGps expr
            eraseGps id
    | :? ExpressionSyntaxElement as exprSyntax ->
        match exprSyntax.Expression with
        | Constant c ->
            eraseGps c
        | Variable v ->
            eraseGps v
        | Object obj ->
            for kvp in obj do
                eraseGps kvp.Key
                eraseGps kvp.Value
        | Expression.Tuple eles ->
            for ele in eles do
                eraseGps ele
        | Expression.Array items ->
            for item in items do
                eraseGps item
        | Operation (left, op, right) ->
            eraseGps left
            eraseGps right
        | Lambda (pars, exp) ->
            for p in pars do
                eraseGps p.Identifier
                match p.Type with
                | LambdaParameterType.TypeDescriptor td ->
                    eraseGps td
                | LambdaParameterType.Inferred -> ()
            eraseGps exp
        | Invocation (id, targs, exps) ->
            eraseGps id
            for ta in targs do eraseGps ta
            for exp in exps do eraseGps exp
        | Ternary (pred, ifT, ifF) ->
            eraseGps pred
            eraseGps ifT
            eraseGps ifF
        | Cast (td, exp) ->
            eraseGps td
            eraseGps exp
        | IsType (exp, td) ->
            eraseGps exp
            eraseGps td
    | :? TypeParameterSyntaxElement as _ -> ()
    | :? TypeDescriptorSyntaxElement as tdSyntax ->
        match tdSyntax.TypeDescriptor with
        | Never -> ()
        | TypeArgument _ -> ()
        | Identified (id, tps) ->
            eraseGps id
            for tp in tps do eraseGps tp
        | Indexed (it, ind) ->
            eraseGps it
            eraseGps ind
        | Array arr ->
            eraseGps arr
        | Tuple tup ->
            for i in tup do eraseGps i
        | Union union ->
            for t in union do eraseGps t
        | Intersection intersection ->
            for i in intersection do eraseGps i
        | Record record ->
            for mem in record do
                eraseGps mem.Identifier
                eraseGps mem.Type
        | Func (targs, parTds, returnTd) ->
            for ta in targs do eraseGps ta
            for ptd in parTds do
                eraseGps ptd
            eraseGps returnTd
        | Literal lit ->
            eraseGps lit
    | :? StatementSyntaxElement as stmtSyntax ->
        match stmtSyntax.Statement with
        | Expression exp ->
            eraseGps exp
        | VariableDeclaration (dt, id, assignVal) ->
            eraseGps id
            eraseGps assignVal
            match dt with
            | TypeDescriptor td ->
                eraseGps td
            | _ -> ()
        | DestructuringVariableDeclaration (ids, exp) ->
            for id in ids do eraseGps id
            eraseGps exp
        | TypeDeclaration (id, tps, assignVal) ->
            eraseGps id
            for tp in tps do eraseGps tp
            eraseGps assignVal
        | Return exp ->
            eraseGps exp
        | Block (stmts) ->
            for stmt in stmts do eraseGps stmt
    | :? MemberSyntaxElement as mem ->
        match mem.Member with
        | Member.TypeDeclaration (_, id, tps, assign) ->
            for tp in tps do eraseGps tp
            eraseGps assign
        | Function (sign, bod) ->
            for (t, id) in sign.Parameters do
                eraseGps t
            for arg in sign.TypeParameters do
                eraseGps arg
            for (_, tc) in sign.TypeConstraints do
                eraseGps tc
            match sign.ReturnType with
            | FunctionReturnType.Inferred -> ()
            | FunctionReturnType.TypeDescriptor td ->
                eraseGps td
            eraseGps bod
            match sign.ReturnType with
            | FunctionReturnType.Inferred -> ()
            | FunctionReturnType.TypeDescriptor td ->
                eraseGps td
    | :? ContextSyntaxElement as con ->
        for b in con.Body do eraseGps b
    | :? TypeConstraintSyntaxElement as tc ->
        match tc.TypeConstraint with
        | IsClosedUnder _ -> ()
        | Extends ts ->
            for td in ts do eraseGps td
    | _ -> ()
