using Ares.Runtime.Table;
using Ares.Runtime.Table.Factory;

namespace Ares.Runtime.Tests.Table;

public class UniformPinnedLookupTableTest
{
    [Fact]
    public void CanTest()
    {
        using var table = TableFactory.Of<NamedTypeInfo>()
            .WithCapacity(TableCapacity.MaxItems(1000))
            .WithUniformlySizedItems()
            .WithPinnedMemory()
            .WithLevelingCache()
            .WithLeveler<NamedType>(new NamedTypeLeveler())
            .Build();
        
        var a = table.Add(new NamedType("hello"));
        var b = table.Add(new NamedType("hi"));
        var c = table.Add(new NamedType("hola"));
        
        Assert.Equal("hello", table[a].TypeName);
        Assert.Equal("hi", table[b].TypeName);
        Assert.Equal("hola", table[c].TypeName);
    }
}