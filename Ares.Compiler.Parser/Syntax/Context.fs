module Ares.Compiler.Parser.Syntax.Context

open System
open Ares.Compiler.Parser.Syntax.Common
open Ares.Compiler.Parser.Syntax.Member

type ContextSyntaxElement(codeSpan: GpsSpan, name: string, body: MemberSyntaxElement list) =
    inherit SyntaxElement(codeSpan, SyntaxElementKind.Context)
    member this.Name
        with get () = name
    member this.Body
        with get () = body
    interface IComparable with
        member this.CompareTo other =
            match other with
            | :? ContextSyntaxElement as p -> (this :> IComparable<_>).CompareTo p
            | _ -> -1

    interface IComparable<ContextSyntaxElement> with
        member this.CompareTo other = other.GetHashCode().CompareTo(this.GetHashCode())
    override this.Equals(obj) =
        match obj with
        | :? ContextSyntaxElement as p -> p.CodeSpan = this.CodeSpan && p.Name = this.Name && p.Body = this.Body && p.Kind = this.Kind
        | _ -> false
    override this.GetHashCode() = hash (this.Name, this.Body, base.GetHashCode())
    override this.ToString() = this.Name.ToString()


