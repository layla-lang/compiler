module Ares.Compiler.Parser.Syntax.TypeConstraint

open System
open Ares.Compiler.Parser.Syntax.Common
open Ares.Compiler.Parser.Syntax.Expression

type TypeConstraint =
    | IsClosedUnder of Operators: string list
    | Extends of TypeDescriptorSyntaxElement list
    static member (!~) (x: TypeConstraint) = TypeConstraintSyntaxElement(GpsSpan.Zeroed, x)

and TypeConstraintSyntaxElement(codeSpan: GpsSpan, typeConstraint: TypeConstraint) =
    inherit SyntaxElement(codeSpan, SyntaxElementKind.Statement)
    member this.TypeConstraint
        with get () = typeConstraint
    interface IComparable with
        member this.CompareTo other =
            match other with
            | :? TypeConstraintSyntaxElement as p -> (this :> IComparable<_>).CompareTo p
            | _ -> -1

    interface IComparable<TypeConstraintSyntaxElement> with
        member this.CompareTo other = other.GetHashCode().CompareTo(this.GetHashCode())
    override this.Equals(obj) =
        match obj with
        | :? TypeConstraintSyntaxElement as p -> p.CodeSpan = this.CodeSpan && p.TypeConstraint = this.TypeConstraint && p.Kind = this.Kind
        | _ -> false
    override this.GetHashCode() = hash (this.TypeConstraint, base.GetHashCode())
    override this.ToString() = this.TypeConstraint.ToString()
