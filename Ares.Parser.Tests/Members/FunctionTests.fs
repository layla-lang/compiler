module Ares.Compiler.Parser.Tests.Members.FunctionTests

open System
open Ares.Compiler.Parser.Syntax
open Ares.Compiler.Parser.Syntax.Expression
open Ares.Compiler.Parser.Syntax.Member
open Ares.Compiler.Parser.Syntax.TypeConstraint
open Ares.Compiler.Parser.Tests.Members.Common
open Xunit
open FsUnit.Xunit

let nonGenericFuncSample =
    """public [[Int, Int]] ToDoubleTuple(Int x) {
         return [[x, x]];
      }"""
let genericFuncSample =
    """public [['T, 'T]] ToDoubleTuple<'T>('T x) {
         return [[x, x]];
      }"""

let inferredReturnTypeSample =
    """public func ToDoubleTuple<'T>('T x) {
         return [[x, x]];
      }"""

let funcWithTypeConstraintsSample = """
given:
  'T extends IComparable;
public [['T, 'T]] ToDoubleTuple<'T>('T x) {
  return [[x, x]];
}
"""

[<Fact>]
let ``Can parse non-generic function`` () =
    let fs: FunctionSignature = {
        Name = "ToDoubleTuple"
        Scope = Scope.Public
        ReturnType = TypeDescriptor(!~TypeDescriptor.Tuple([
            !~Identified(!~Simple("Int"), [])
            !~Identified(!~Simple("Int"), [])
        ]))
        TypeParameters = []
        Parameters = [ !~Identified(!~Simple("Int"), []), "x" ]
        TypeConstraints = []
    }
    Assert.Equal(Function(
        fs, !~Statement.Return(
            !~Expression.Tuple([
                !~Variable(!~Simple("x"))
                !~Variable(!~Simple("x"))
            ]))), parseMember nonGenericFuncSample)

[<Fact>]
let ``Can parse function member without type constraints`` () =
    let fs: FunctionSignature = {
        Name = "ToDoubleTuple"
        Scope = Scope.Public
        ReturnType = TypeDescriptor(!~TypeDescriptor.Tuple([ !~TypeArgument("'T"); !~TypeArgument("'T") ]))
        TypeParameters = [ !~{ Identifier = "'T"; Level = 1 } ]
        Parameters = [ (!~TypeArgument("'T"), "x") ]
        TypeConstraints = []
    }
    Assert.Equal(Function(
        fs, !~Statement.Return(
            !~Expression.Tuple([
                !~Variable(!~Simple("x"))
                !~Variable(!~Simple("x"))
            ]))), parseMember genericFuncSample)

[<Fact>]
let ``Can parse function with inferred return type`` () =
    let fs: FunctionSignature = {
        Name = "ToDoubleTuple"
        Scope = Scope.Public
        ReturnType = Inferred
        TypeParameters = [ !~{ Identifier = "'T"; Level = 1 } ]
        Parameters = [ (!~TypeArgument("'T"), "x") ]
        TypeConstraints = []
    }
    Assert.Equal(Function(
        fs, !~Statement.Return(
            !~Expression.Tuple([
                !~Variable(!~Simple("x"))
                !~Variable(!~Simple("x"))
            ]))), parseMember inferredReturnTypeSample)
    

[<Fact>]
let ``Can parse function member with type constraints`` () =
    let tcExtendsComparable = !~Extends([ !~TypeDescriptor.Identified(!~Simple("IComparable"), []) ])

    let Tuple2OfT_T = TypeDescriptor(!~TypeDescriptor.Tuple([ !~TypeArgument("'T"); !~TypeArgument("'T") ]))
    let TypeParamT = !~{ Identifier = "'T"; Level = 1 }
    let result =
        match (parseMember (funcWithTypeConstraintsSample.Trim())) with
        | Function (signature, _) ->
            signature
        | _ -> raise (Exception("Not a function"))
        
    result.Name |> should equal "ToDoubleTuple"
    result.Scope |> should equal Scope.Public
    result.ReturnType |> should equal Tuple2OfT_T
    result.TypeParameters |> should equal [ TypeParamT ]  
    result.Parameters |> should equal [ (!~TypeArgument("'T"), "x") ]
    result.TypeConstraints |> should equal [ ("'T", tcExtendsComparable) ]