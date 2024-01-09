module Ares.Compiler.Parser.Syntax.Statement

open System
open Ares.Compiler.Parser.Syntax.Common
open Ares.Compiler.Parser.Syntax.Expression

type public DeclaredType =
    | Inferred
    | TypeDescriptor of TypeDescriptorSyntaxElement
    
type VariableDeclaration = {
    Identifier: string
    Type: DeclaredType
    Value: ExpressionSyntaxElement
}

type TypeDeclaration = {
    Identifier: string
    TypeParameters: TypeParameterSyntaxElement list
    TypeEquivalency: TypeDescriptorSyntaxElement
}

type StmtStx =
    | VariableDeclaration of DeclaredType: DeclaredType * Identifier: IdentifierSyntaxElement * AssignedValue: ExpressionSyntaxElement
    | DestructuringVariableDeclaration of Identifiers: IdentifierSyntaxElement list * AssignedValue: ExpressionSyntaxElement
    | TypeDeclaration of IdentifierSyntaxElement * TypeParameterSyntaxElement list * TypeDescriptorSyntaxElement
    | Expression of ExpressionSyntaxElement
    | Return of ExpressionSyntaxElement
    | Block of StatementSyntaxElement list
    static member (!~) (x: StmtStx) = StatementSyntaxElement(GpsSpan.Zeroed, x)

and StatementSyntaxElement(codeSpan: GpsSpan, stmt: StmtStx) =
    inherit SyntaxElement(codeSpan, SyntaxElementKind.Statement)
    member this.Statement
        with get () = stmt
    interface IComparable with
        member this.CompareTo other =
            match other with
            | :? StatementSyntaxElement as p -> (this :> IComparable<_>).CompareTo p
            | _ -> -1

    interface IComparable<StatementSyntaxElement> with
        member this.CompareTo other = other.GetHashCode().CompareTo(this.GetHashCode())
    override this.Equals(obj) =
        match obj with
        | :? StatementSyntaxElement as p -> p.CodeSpan = this.CodeSpan && p.Statement = this.Statement && p.Kind = this.Kind
        | _ -> false
    override this.GetHashCode() = hash (this.Statement, base.GetHashCode())
    override this.ToString() = this.Statement.ToString()
