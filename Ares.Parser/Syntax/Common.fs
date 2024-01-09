module Ares.Compiler.Parser.Syntax.Common

open System

[<StructuralEquality; NoComparison>]
type SourceGps =
  { Location: string
    Index: int64
    Line: int64
    Column: int64 }

  static member internal Zeroed =
    { Location = ""
      Index = 0
      Line = 0
      Column = 0 }

[<StructuralEquality; NoComparison>]
type GpsSpan =
  { Start: SourceGps
    End: SourceGps }
  static member internal Zeroed =
    { Start = SourceGps.Zeroed
      End = SourceGps.Zeroed }    

type SyntaxElementKind =
    |Value=0
    |Identifier=1
    |Expression=2
    |TypeDescriptor=3
    |TypeParameter=4
    |Statement=5
    |Member=6
    |Context=7

[<AbstractClass>]
type SyntaxElement(codeSpan: GpsSpan, kind: SyntaxElementKind) =
    let mutable _codeSpan = codeSpan
    member this.CodeSpan
        with get () = _codeSpan
    member this.Kind
        with get () = kind
    
    member this.EraseGpsForTesting() =
        _codeSpan <- GpsSpan.Zeroed
    
    interface IComparable with
        member this.CompareTo other =
            match other with
            | :? SyntaxElement as p -> (this :> IComparable<_>).CompareTo p
            | _ -> -1
    interface IComparable<SyntaxElement> with
        member this.CompareTo other = other.GetHashCode().CompareTo(this.GetHashCode())
    interface IEquatable<SyntaxElement> with
        override this.Equals (other: SyntaxElement) =
            other.CodeSpan = this.CodeSpan && other.GetHashCode() = this.GetHashCode()

    override this.Equals(obj) =
            match obj with
            | :? SyntaxElement as p ->
                p.CodeSpan = this.CodeSpan && p.GetHashCode() = this.GetHashCode()
            | _ -> false
    override this.GetHashCode() = hash (this.Kind, this.CodeSpan.GetHashCode())
    override this.ToString() = this.CodeSpan.ToString()