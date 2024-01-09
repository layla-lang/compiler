using Ares.Compiler.Analysis.Entities;
using Ares.Compiler.Analysis.Entities.Types;
using Ares.Compiler.Analysis.Tables;

namespace Ares.Compiler.Tests.Analysis.Tables;

public class ScopedTableTest
{
    [Fact]
    public void NewScopedCopiesCurrentIntoKnown()
    {
        var tbl = new ScopedTable<KnownValue>(new Scope("module"));
        Assert.Equal(0, tbl.Count);
        Assert.Equal(0, tbl.CurrentScopeCount);
        Assert.Equal(0, tbl.OuterScopeCount);

        var kv = new KnownValue("hi", TypeEntity.Float, null);
        tbl.Add(kv);
        
        Assert.Equal(1, tbl.Count);
        Assert.Equal(1, tbl.CurrentScopeCount);
        Assert.Equal(0, tbl.OuterScopeCount);
        
        Assert.Equal(kv, tbl["hi"]!.Value.Result);
        var subscope = tbl.NewScope("lambda");
        Assert.Equal(1, subscope.Count);
        Assert.Equal(0, subscope.CurrentScopeCount);
        Assert.Equal(1, subscope.OuterScopeCount);

        var kvResult = subscope["hi"]!.Value;
        Assert.Equal(kv, kvResult.Result);
        Assert.Equal(new Scope("module"), kvResult.Scope);
    }

    [Fact]
    public void ScopedTablesAreIsolated()
    {
        var tbl = new ScopedTable<KnownValue>(new Scope("module"));
        var subscoped = tbl.NewScope("lambda");
        subscoped.Add(new KnownValue("hi", TypeEntity.Bool, null));
        Assert.Equal(0, tbl.Count);
        Assert.Equal(1, subscoped.Count);
    }
}