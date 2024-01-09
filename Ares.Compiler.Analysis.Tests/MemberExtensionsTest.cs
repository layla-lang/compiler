using Ares.Compiler.Analysis.Entities;
using Ares.Compiler.Analysis.Entities.Types;
using Ares.Compiler.Checkpoints;
using Xunit;

namespace Ares.Compiler.Tests.Analysis;

public class MemberExtensionsTest
{
    [Fact]
    public void ScopedContextAttachedToFuncMembers()
    {
        var _Square = new CodeCheckpoint("Square");
        var result = SyntaxUtilities.InspectSource(
            $$"""
              context Sample;

              public func {{_Square}}<'T>('T x) {
                return x;
              }
              """);
        var square = result.Context.Methods.GetNearest(_Square)!;
        var te = new TypeArgEntity("'T", null);
        Assert.NotNull(square.ScopedContext);
        Assert.Equal(te, square.ScopedContext.Values["x"]!.Value.Result.Type);
    }
    [Fact]
    public void InfersFuncReturnType()
    {
        var _Square = new CodeCheckpoint("Square");
        var result = SyntaxUtilities.InspectSource(
            $$"""
                      context Sample;

                      public func {{_Square}}<'T>('T x) {
                        return x;
                      }
                      """);
        var square = result.Context.Methods.GetNearest(_Square)!;
        Assert.IsType<TypeArgEntity>(square.ReturnType);
        Assert.Equal("'T", ((TypeArgEntity)square.ReturnType).ParameterName);
    }

    [Fact]
    public void InjectsAliasedParameterType()
    {
        var _First = new CodeCheckpoint("First");
        var result = SyntaxUtilities.InspectSource(
            $$"""
              context Scratchpad;
              
              public type ToStr<'T> = 'T;
              public type StrToStr = ToStr<String>;
              
              public func {{_First}}(ToStr<String> hi) {
                return 4;
              }
              
              scratch {
                var s = First("hi");
              }
              """);
        var First = result.Context.Methods.GetNearest(_First)!;
        var p = (AliasedTypeEntity)First.ParameterTypes[0];
        Assert.Equal(TypeEntity.String,  p.ResolveReference());
    }
}