module rec Ares.Compiler.Parser.Syntax.Expression

open System
open Ares.Compiler.Parser.Syntax.Common
open Ares.Compiler.Parser.Syntax.Value

type Operator =
    | Addition
    | Subtraction
    | Multiplication
    | Division
    | Modulus
    | Power
    | Gt
    | Gte
    | Lt
    | Lte
    | Equals
    | NotEquals
    | BoolAnd
    | BoolOr

type public LambdaParameterType =
    | Inferred
    | TypeDescriptor of TypeDescriptorSyntaxElement
    
type LambdaParameter = {
    Identifier: IdentifierSyntaxElement
    Type: LambdaParameterType
}

type RecordMember = {
    Identifier: IdentifierSyntaxElement
    Type: TypeDescriptorSyntaxElement
}

type TypeParameter =
  { Identifier: string; Level: int; }
  static member (!~) (x: TypeParameter) = TypeParameterSyntaxElement(GpsSpan.Zeroed, x)

and Expression =
    | Constant of ValueSyntaxElement
    | Variable of IdentifierSyntaxElement
    | Invocation of Identifier: IdentifierSyntaxElement * TypeArguments: TypeDescriptorSyntaxElement list * Parameters: ExpressionSyntaxElement list
    | Operation of (ExpressionSyntaxElement * Operator * ExpressionSyntaxElement)
    | Ternary of Predicate: ExpressionSyntaxElement * IfTrue: ExpressionSyntaxElement * IfFalse: ExpressionSyntaxElement
    | Lambda of Parameters: LambdaParameter list * Expression: ExpressionSyntaxElement
    | Cast of NewType: TypeDescriptorSyntaxElement * Expression: ExpressionSyntaxElement
    | IsType of Expression: ExpressionSyntaxElement * TypeDescriptor: TypeDescriptorSyntaxElement
    | Object of Map<IdentifierSyntaxElement, ExpressionSyntaxElement>
    | Tuple of ExpressionSyntaxElement list
    | Array of ExpressionSyntaxElement list
    static member (!~) (exp: Expression) = ExpressionSyntaxElement(GpsSpan.Zeroed, exp)

and TypeDescriptor =
    | Never
    | Any
    | TypeArgument of string
    | Identified of Identifier: IdentifierSyntaxElement * TypeArguments: TypeDescriptorSyntaxElement list
    | Literal of ValueSyntaxElement
    | Union of TypeDescriptorSyntaxElement list
    | Intersection of TypeDescriptorSyntaxElement list
    | Record of RecordMember list
    | Array of TypeDescriptorSyntaxElement
    | Indexed of IndexedType: TypeDescriptorSyntaxElement * Indexer: TypeDescriptorSyntaxElement
    | Tuple of TypeDescriptorSyntaxElement list
    | Func of TypeParameters: TypeParameterSyntaxElement list * Parameters: TypeDescriptorSyntaxElement list * ReturnType: TypeDescriptorSyntaxElement
    static member (!~) (td : TypeDescriptor) = TypeDescriptorSyntaxElement(GpsSpan.Zeroed, td)

and Identifier =
    | Simple of string
    | IndexAccess of IdentifierSyntaxElement * ExpressionSyntaxElement
    | MemberAccess of IdentifierSyntaxElement list * IdentifierSyntaxElement
    static member (!~) (id : Identifier) = IdentifierSyntaxElement(GpsSpan.Zeroed, id)
    
and ExpressionSyntaxElement(codeSpan: GpsSpan, exp: Expression) =
    inherit SyntaxElement(codeSpan, SyntaxElementKind.Expression)
    member this.Expression
        with get () = exp
    interface IComparable with
        member this.CompareTo other =
            match other with
            | :? ExpressionSyntaxElement as p -> (this :> IComparable<_>).CompareTo p
            | _ -> -1

    interface IComparable<ExpressionSyntaxElement> with
        member this.CompareTo other = other.GetHashCode().CompareTo(this.GetHashCode()) 
    override this.Equals(obj) =
        match obj with
        | :? ExpressionSyntaxElement as p -> p.CodeSpan = this.CodeSpan && p.Expression = this.Expression && p.Kind = this.Kind
        | _ -> false
    override this.GetHashCode() = hash (this.Expression, base.GetHashCode())
    override this.ToString() = this.Expression.ToString()

and TypeDescriptorSyntaxElement(codeSpan: GpsSpan, td: TypeDescriptor) =
    inherit SyntaxElement(codeSpan, SyntaxElementKind.TypeDescriptor)
    member this.TypeDescriptor
        with get () = td
    interface IComparable with
        member this.CompareTo other =
            match other with
            | :? TypeDescriptorSyntaxElement as p -> (this :> IComparable<_>).CompareTo p
            | _ -> -1

    interface IComparable<TypeDescriptorSyntaxElement> with
        member this.CompareTo other = other.GetHashCode().CompareTo(this.GetHashCode())     
    override this.Equals(obj) =
        match obj with
        | :? TypeDescriptorSyntaxElement as p -> p.CodeSpan = this.CodeSpan && p.TypeDescriptor = this.TypeDescriptor && p.Kind = this.Kind
        | _ -> false
    override this.GetHashCode() = hash (this.TypeDescriptor, base.GetHashCode())
    override this.ToString() = this.TypeDescriptor.ToString()

and IdentifierSyntaxElement(codeSpan: GpsSpan, id: Identifier) =
    inherit SyntaxElement(codeSpan, SyntaxElementKind.Identifier)
    member this.Identifier
        with get () = id

    interface IComparable with
        member this.CompareTo other =
            match other with
            | :? IdentifierSyntaxElement as p -> (this :> IComparable<_>).CompareTo p
            | _ -> -1
    interface IComparable<IdentifierSyntaxElement> with
        member this.CompareTo other = other.GetHashCode().CompareTo(this.GetHashCode())
    override this.Equals(obj) =
        match obj with
        | :? IdentifierSyntaxElement as p -> p.CodeSpan = this.CodeSpan && p.Identifier = this.Identifier && p.Kind = this.Kind
        | _ -> false
    override this.GetHashCode() = hash (this.Identifier, base.GetHashCode())
    override this.ToString() = this.Identifier.ToString()

and TypeParameterSyntaxElement(codeSpan: GpsSpan, tp: TypeParameter) =
    inherit SyntaxElement(codeSpan, SyntaxElementKind.TypeParameter)
    member this.TypeParameter
        with get () = tp
    interface IComparable with
        member this.CompareTo other =
            match other with
            | :? TypeParameterSyntaxElement as p -> (this :> IComparable<_>).CompareTo p
            | _ -> -1

    interface IComparable<TypeParameterSyntaxElement> with
        member this.CompareTo other = other.GetHashCode().CompareTo(this.GetHashCode())     
    override this.Equals(obj) =
        match obj with
        | :? TypeParameterSyntaxElement as p -> p.CodeSpan = this.CodeSpan && p.TypeParameter.Identifier = this.TypeParameter.Identifier && p.Kind = this.Kind
        | _ -> false
    override this.GetHashCode() = hash (this.TypeParameter.Identifier, base.GetHashCode())
    override this.ToString() = this.TypeParameter.Identifier.ToString()