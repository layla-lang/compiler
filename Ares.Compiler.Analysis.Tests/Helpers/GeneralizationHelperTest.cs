using Ares.Compiler.Analysis;
using Ares.Compiler.Analysis.Entities;
using Ares.Compiler.Analysis.Entities.Types;
using Ares.Compiler.Analysis.Helpers;
using Xunit;

namespace Ares.Compiler.Tests.Analysis.Helpers;

public class GeneralizationHelperTest
{
    private readonly SourceContext context;
    [Fact]
    public void GeneralizesToNeverWhenCalledWithNoTypes()
    {
        var generalized = GeneralizationHelper.Generalize(context);
        Assert.Equal(TypeEntity.Never, generalized);
    }
    [Fact]
    public void GeneralizesToAnyWhenCalledWithAnAnyType()
    {
        var generalized = GeneralizationHelper.Generalize(context, TypeEntity.Bool, TypeEntity.Int, TypeEntity.Any);
        Assert.Equal(TypeEntity.Any, generalized);
    }
    [Fact]
    public void GeneralizesSingleTypeToItself()
    {
        var te = new TypeArgEntity("'T", null);
        var generalized = GeneralizationHelper.Generalize(context, te, null);
        Assert.Equal(te, generalized);
    }
    
    [Fact]
    public void DedupesIdenticalTypes()
    {
        var generalized = GeneralizationHelper.Generalize(context, TypeEntity.Int, TypeEntity.Int);
        Assert.Equal(TypeEntity.Int, generalized);
    }
    
    [Fact]
    public void GeneralizesToCommonSupertype()
    {
        var left = new TypeArgEntity("'T1", null);
        var right = new TypeArgEntity("'T2", null);
        
        var t3 = new TypeArgEntity("'T3", null);
        var rightLeftMiddleNode = context.Universe[right];
        var rightNode = context.Universe[right];
        context.Universe.AddTypeExtendsFact(
            t3,
            rightLeftMiddleNode.Type);
        
        var generalized = GeneralizationHelper.Generalize(context, new []
        {
            rightLeftMiddleNode.Type,
            rightNode.Type
        });
        Assert.Equal(rightLeftMiddleNode.Type, generalized);
    }
}