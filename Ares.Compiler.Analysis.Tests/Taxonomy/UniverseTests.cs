using Ares.Compiler.Analysis.Entities;
using Ares.Compiler.Analysis.Entities.Types;
using Ares.Compiler.Analysis.Tables;
using Ares.Compiler.Analysis.Taxonomy;

namespace Ares.Compiler.Tests.Analysis.Taxonomy;

public class UniverseTests
{
    private readonly Universe universe = new Universe(new Scope("global"));
    
    [Fact]
    public void TypesInitiallyParentedByAny()
    {
        var lca = universe.LowestCommonAncestor(TypeEntity.Int, TypeEntity.String);
        Assert.Equal(TypeEntity.Any, lca);
        Assert.Equal(TypeEntity.Any, universe.GetParent(TypeEntity.Int));
        Assert.Equal(TypeEntity.Any, universe.GetParent(TypeEntity.String));
    }
    
    [Fact]
    public void TypesCanBeReParented()
    {
        var targ1 = new TypeArgEntity("'T1", null);
        var beforeFact = universe.GetParent(targ1);
        
        Assert.Equal(TypeEntity.Any, beforeFact);
        
        var targ2 = new TypeArgEntity("'T1", null);
        universe.AddTypeExtendsFact(targ1, targ2);
        var afterFact = universe.GetParent(targ1);
        
        Assert.Equal(targ2, afterFact);
        
        var lca = universe.LowestCommonAncestor(TypeEntity.Int, TypeEntity.String);
        Assert.Equal(TypeEntity.Any, lca);
    }

    [Fact]
    public void FindsLca()
    {
        var tl = new TypeArgEntity("'TL", null);
        var tr = new TypeArgEntity("'TR", null);
        var topLeft = universe[tl];
        var topRight = universe[tr];
        
        var mrl = new TypeArgEntity("`Mlr", null);
        var mrr = new TypeArgEntity("`Mlr", null);
        universe[mrl].ReParent(topRight);
        universe[mrr].ReParent(topRight);
        
        Assert.Equal(tr, universe.LowestCommonAncestor(mrl, mrr));
    }
}