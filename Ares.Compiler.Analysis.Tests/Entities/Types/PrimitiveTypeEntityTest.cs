using Ares.Compiler.Analysis.Entities.Types;

namespace Ares.Compiler.Tests.Analysis.Entities.Types;

public class PrimitiveTypeEntityTest
{
    [Fact]
    public void PrimitiveTypesEqualityWorksCorrectly()
    {
        var p1 = new PrimitiveTypeEntity("hi", null);
        var p2 = new PrimitiveTypeEntity("hi", null);
        var p3 = new PrimitiveTypeEntity("hello", null);
        
        Assert.Equal(p1, p2);
        Assert.NotEqual(p2, p3);
        var ls = new List<PrimitiveTypeEntity>(new[] { p1, p2, p3 }).Distinct().ToList();
        Assert.Equal(2, ls.Count);
        Assert.Contains(ls, (f) => f == p1);
        Assert.Contains(ls, (f) => f == p3);
    }
    
    [Fact]
    public void PrimitiveTypeEqualityWorksCorrectly()
    {
        var p = new PrimitiveTypeEntity("String", null);
        
        Assert.True(p == TypeEntity.String);
        Assert.True(TypeEntity.String == p);
        Assert.Equal(TypeEntity.String, p);
        Assert.NotEqual(TypeEntity.Int, p);
    }
}