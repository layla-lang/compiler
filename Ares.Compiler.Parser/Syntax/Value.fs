module Ares.Compiler.Parser.Syntax.Value

open System
open Ares.Compiler.Parser.Syntax.Common

type Literal =
    | IntLiteral of int
    | FloatLiteral of float
    | BoolLiteral of bool
    | StringLiteral of string
    static member (!~) (x: Literal) = ValueSyntaxElement(GpsSpan.Zeroed, x)

and ValueSyntaxElement(codeSpan: GpsSpan, lit: Literal) =
    inherit SyntaxElement(codeSpan, SyntaxElementKind.Value)
    member this.Literal
        with get () = lit

    interface IComparable with
        member this.CompareTo other =
            match other with
            | :? ValueSyntaxElement as p -> (this :> IComparable<_>).CompareTo p
            | _ -> -1
    interface IComparable<ValueSyntaxElement> with
        member this.CompareTo other = other.GetHashCode().CompareTo(this.GetHashCode())
    override this.Equals(obj) =
        match obj with
        | :? ValueSyntaxElement as p ->
            p.CodeSpan = this.CodeSpan && p.Literal = this.Literal && p.Kind = this.Kind
        | _ -> false
    override this.GetHashCode() =
        hash (this.Literal, base.GetHashCode())
    override this.ToString() = this.Literal.ToString()

