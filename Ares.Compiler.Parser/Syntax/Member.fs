module Ares.Compiler.Parser.Syntax.Member

open System
open Ares.Compiler.Parser.Syntax.Common
open Ares.Compiler.Parser.Syntax.Expression
open Ares.Compiler.Parser.Syntax.Statement
open Ares.Compiler.Parser.Syntax.TypeConstraint

type Scope =
    | Public
    | Internal
    | Protected
    | Private

type public FunctionReturnType =
    | Inferred
    | TypeDescriptor of TypeDescriptorSyntaxElement
    
type FunctionSignature = {
    Name: string
    Scope: Scope
    TypeParameters: TypeParameterSyntaxElement list
    Parameters: (TypeDescriptorSyntaxElement * string) list
    ReturnType: FunctionReturnType
    TypeConstraints: (string * TypeConstraintSyntaxElement) list
}

type Member =
    | Scratch of Body: StatementSyntaxElement
    | TypeDeclaration of Scope: Scope * Name: string * TypeParameters: TypeParameterSyntaxElement list * TypeDescriptor: TypeDescriptorSyntaxElement
    | Function of Signature: FunctionSignature * Body: StatementSyntaxElement
    static member (!~) (x: Member) = MemberSyntaxElement(GpsSpan.Zeroed, x)

and MemberSyntaxElement(cs: GpsSpan, mem: Member) =
    inherit SyntaxElement(cs, SyntaxElementKind.Member)
    member this.Member
        with get () = mem
        
    interface IComparable with
        member this.CompareTo other =
            match other with
            | :? MemberSyntaxElement as p -> (this :> IComparable<_>).CompareTo p
            | _ -> -1

    interface IComparable<MemberSyntaxElement> with
        member this.CompareTo other = other.GetHashCode().CompareTo(this.GetHashCode()) 
    override this.Equals(obj) =
        match obj with
        | :? MemberSyntaxElement as p ->
            p.CodeSpan = this.CodeSpan &&
            p.Member = this.Member
        | _ -> false
    override this.GetHashCode() = hash (this.Member, base.GetHashCode())
    override this.ToString() = this.Member.ToString()