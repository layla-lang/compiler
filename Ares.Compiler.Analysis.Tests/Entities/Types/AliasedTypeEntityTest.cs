using System.Collections.Immutable;
using Ares.Compiler.Analysis.Entities.Types;
using Ares.Compiler.Analysis.Entities.Types.TypeArgs;
using Ares.Compiler.Tokens;

namespace Ares.Compiler.Tests.Analysis.Entities.Types;

public class AliasedTypeEntityTest
{
    [Fact]
    public void CanSubstituteTypeArgsIntoAliasedType()
    {
        var tp1 = new TypeArgEntity("'T1", null);
        var tp2 = new TypeArgEntity("'T2", null);
        var arrType = new TupleTypeEntity(ImmutableList.Create<TypeEntity>()
            .Add(tp1)
            .Add(tp2), null);
        var genTypeAlias = new AliasedTypeEntity("X", arrType,
            ImmutableList.Create<TypeParam>()
                .Add(new TypeParam(tp1.ParameterName, ImmutableList<TypeConstraintToken>.Empty))
                .Add(new TypeParam(tp2.ParameterName)),
            null);
        
        Assert.Equal(2, genTypeAlias.Arity);
        Assert.Equal(0, genTypeAlias.ClosedTypeArgs.Count());
        Assert.Equal(2, genTypeAlias.OpenTypeArgs.Count());

        var sub1 = (AliasedTypeEntity)genTypeAlias.ProvideTypeArgument(
            tp1, TypeEntity.Int);
        var sub1T = (TupleTypeEntity)sub1.ReferencedTypeEntity;
        
        Assert.Equal(1, sub1.Arity);
        Assert.Equal(1, sub1.ClosedTypeArgs.Count());
        Assert.Equal(new ClosedTypeArg(tp1.ParameterName, TypeEntity.Int), sub1.ClosedTypeArgs.First());
        Assert.Equal(1, sub1.OpenTypeArgs.Count());
        Assert.Equal(TypeEntity.Int, sub1T.ElementTypeEntities[0]);
        Assert.Equal(tp2, sub1T.ElementTypeEntities[1]);
        
        var sub2 = (AliasedTypeEntity)sub1.ProvideTypeArgument(
            tp2, TypeEntity.String);
        var sub2T = (TupleTypeEntity)sub2.ReferencedTypeEntity;
        
        Assert.Equal(0, sub2.Arity);
        Assert.Equal(2, sub2.ClosedTypeArgs.Count());
        Assert.Equal(new ClosedTypeArg(tp2.ParameterName, TypeEntity.String), sub2.ClosedTypeArgs.Last());
        Assert.Equal(0, sub2.OpenTypeArgs.Count());
        Assert.Equal(TypeEntity.String, sub2T.ElementTypeEntities[1]);
    }
    
    [Fact]
    public void GenericSubstitutionWorks()
    {
        var tp = new TypeArgEntity("'T", null);
        var arrType = new ArrayTypeEntity(tp, null);
        var genTypeAlias = new AliasedTypeEntity("X", arrType,
            ImmutableList.Create<TypeParam>().Add(new TypeParam("'T")),
            null);

        var provided = (AliasedTypeEntity)genTypeAlias.ProvideTypeArgument(tp, TypeEntity.Int);
        Assert.Equal(new ArrayTypeEntity(TypeEntity.Int, null), provided.ReferencedTypeEntity);
    }
}