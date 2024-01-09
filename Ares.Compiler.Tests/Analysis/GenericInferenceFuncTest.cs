using Ares.Compiler.Analysis.Entities;
using Ares.Compiler.Analysis.Entities.Types;
using Ares.Compiler.Checkpoints;

namespace Ares.Compiler.Tests.Analysis;

public class GenericInferenceFuncTest
{
    [Fact]
    public void CanSubstituteTypeArgsForFuncMember()
    {
        string code = """
                      context Sample;
                      
                      public 'T Square<'T>('T x) {
                        return x;
                      }
                      """;
        var result = SyntaxUtilities.InspectSource(code);
        var squareMethodType = result.Context!.Methods!["Square`1('T)"]!.Value.Result;
        Assert.NotNull(squareMethodType);
        Assert.Equal(1, squareMethodType.Arity);
        Assert.Equal("'T", squareMethodType.OpenTypeArgs.First().Identifier);
        Assert.Empty(squareMethodType.ClosedTypeArgs);

        var closedMethodType = squareMethodType.ProvideTypeArgument(
            new TypeArgEntity("'T", null),
            TypeEntity.Bool);
        
        Assert.Equal(TypeEntity.Bool, closedMethodType.ReturnType);
        Assert.Equal(0, closedMethodType.Arity);
        Assert.Equal("'T", closedMethodType.ClosedTypeArgs.First().Identifier);
        Assert.Equal(TypeEntity.Bool, closedMethodType.ClosedTypeArgs.First().ClosingType);
        Assert.Empty(closedMethodType.OpenTypeArgs);
    }

    [Fact]
    public void CanSubstituteTypesIntoArray()
    {
        var result = SyntaxUtilities.InspectSource(
            $$"""
              context Scratchpad;

              public 'T First<'T>('T[] arr) {
                return arr[0];
              }

              scratch {
                var s = First([2, 3]);
              }
              """);
        var s = result.Context.Values["s"]!.Value.Result!;
        Assert.Equal(TypeEntity.Int, s.Type);
    }
    
    [Fact]
    public void CanSubstituteTypesIntoTuple()
    {
        var result = SyntaxUtilities.InspectSource(
            $$"""
              context Scratchpad;

              public func First<'T,'U>([['T, 'U]] tup) {
                var [[first, last]] = tup;
                return [[last, first]];
              }

              scratch {
                var s = First([[2, "Hi"]]);
              }
              """);
        var s = result.Context.Values["s"]!.Value.Result!;
        var exp = new TupleTypeEntity(new[]
        {
            TypeEntity.String,
            TypeEntity.Int
        }, null);
        Assert.Equal(exp, s.Type);
    }

    [Fact]
    public void CanDetermineInvocationResult()
    {
        string code = """
                      context Sample;

                      public 'T Square<'T>('T x) {
                        return x;
                      }
                      
                      scratch {
                        var x = Square<Int>(3);
                      }
                      """;
        var result = SyntaxUtilities.InspectSource(code);
        var xVal = result.Context!.Values["x"]!.Value.Result;
        Assert.NotNull(xVal);
    }
}