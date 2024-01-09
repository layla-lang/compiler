using System.Collections.Immutable;
using Ares.Compiler.Analysis.Entities.Types;
using Xunit;

namespace Ares.Compiler.Tests.Analysis.Entities.Types;

public class TupleTypeEntityTest
{
    [Fact]
    public void TupleTypesEqualityWorksCorrectly()
    {
        var te = new TypeArgEntity("'T", null);
        
        var tup1 = new TupleTypeEntity(ImmutableArray.Create<TypeEntity>()
            .Add(te).Add(te), null);
        var tup2 = new TupleTypeEntity(ImmutableList.Create<TypeEntity>()
            .Add(te).Add(te), null);
        
        Assert.Equal(tup1, tup2);
        Assert.Equal(tup1.GetHashCode(), tup2.GetHashCode());
        
        var deduped = new List<TupleTypeEntity> { tup1, tup2 }.Distinct().ToList();
        Assert.Single(deduped);
        Assert.Equal(tup1, deduped[0]);
    }
}