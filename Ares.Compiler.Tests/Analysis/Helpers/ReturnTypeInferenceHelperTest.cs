using System.Collections.Immutable;
using Ares.Compiler.Analysis.Entities;
using Ares.Compiler.Analysis.Entities.Types;
using Ares.Compiler.Checkpoints;

namespace Ares.Compiler.Tests.Analysis.Helpers;

public class ReturnTypeInferenceHelperTest
{
    [Fact]
    public void InfersReturnTypeAcrossTernary()
    {
        var _Sort2 = new CodeCheckpoint("Sort2");
        var result = SyntaxUtilities.InspectSource(
            $$"""
              context Scratchpad;
              
              given:
                'T is closed under >;
              public func {{_Sort2}}<'T>('T x1, 'T x2) {
                return x1 > x2 ? [[x2, x1]] : [[x1, x2]];
              }
              """);
        var Sort2 = result.Context.Methods.GetNearest(_Sort2)!;
        var te = new TypeArgEntity("'T", null);
        var tup = new TupleTypeEntity(ImmutableList.Create<TypeEntity>().Add(te).Add(te), null);
        Assert.Equal(tup, Sort2.ReturnType);
    }

    [Fact]
    public void InfersReturnTypeGenericTypeAndDestructured()
    {
        var result = SyntaxUtilities.InspectSource(
            $$"""
              context Scratchpad;
              
              given:
                'T is closed under >;
              public func Sort2<'T>('T x1, 'T x2) {
                return x1 > x2 ? [[x2, x1]] : [[x1, x2]];
              }
              
              scratch {
                var [[small, big]] = Sort2(7, 5);
              }
              """);
        var smallV = result.Context.Values["small"]!.Value.Result;
        var bigV = result.Context.Values["big"]!.Value.Result;
        
        Assert.Equal(TypeEntity.Int, smallV.Type);
        Assert.Equal(TypeEntity.Int, bigV.Type);
    }
}