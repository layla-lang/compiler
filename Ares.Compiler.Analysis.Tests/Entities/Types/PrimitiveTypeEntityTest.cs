using Ares.Compiler.Analysis.Entities.Types;
using Xunit;

namespace Ares.Compiler.Tests.Analysis.Entities.Types;

public class PrimitiveTypeEntityTest
{
    [Fact]
    public void PrimitiveTypesEqualityWorksCorrectly()
    {
        var p1 = TypeEntity.Bool;
        var p2 = TypeEntity.Byte;
        var p3 = TypeEntity.String;
        
        Assert.Equal(p1, p2);
        Assert.NotEqual(p2, p3);
        var ls = new List<TypeEntity>(new[] { p1, p2, p3 })
            .Cast<PrimitiveTypeEntity>().Distinct().ToList();
        Assert.Equal(2, ls.Count);
        Assert.Contains(ls, (f) => f == p1);
        Assert.Contains(ls, (f) => f == p3);
    }
    
    [Fact]
    public void PrimitiveTypeEqualityWorksCorrectly()
    {
        var p = TypeEntity.String;
        
        Assert.True(p == TypeEntity.String);
        Assert.True(TypeEntity.String == p);
        Assert.Equal(TypeEntity.String, p);
        Assert.NotEqual(TypeEntity.Int, p);
    }
}