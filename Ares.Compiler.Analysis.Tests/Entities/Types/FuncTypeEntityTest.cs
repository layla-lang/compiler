using Ares.Compiler.Analysis.Entities.Types;
using Ares.Compiler.Analysis.Tables;

namespace Ares.Compiler.Tests.Analysis.Entities.Types;

public class FuncTypeEntityTest
{
    [Fact]
    public void FuncTypesEqualityWorksCorrectly()
    {
        var pars = new List<TypeEntity>(new[] { TypeEntity.Int, TypeEntity.String });

        var f1 = new FuncTypeEntity(pars, TypeEntity.Int, null);
        var f2 = new FuncTypeEntity(pars, TypeEntity.Int, null);
        Assert.Equal(f1, f2);
        
        var f3 = new FuncTypeEntity(pars, TypeEntity.String, null);
        Assert.NotEqual(f1, f3);
        
        var f4 =new FuncTypeEntity(pars.Skip(1), TypeEntity.Int, null);
        Assert.NotEqual(f1, f4);
        
        var ls = new List<FuncTypeEntity>(new[] { f1, f2, f3, f4 }).Distinct().ToList();
        Assert.Equal(3, ls.Count);
    }
    
    [Fact]
    public void CanSubstituteTypeArgsIntoFuncTypes()
    {
        string code = """
                      type ArrayDoubler<'T> = ('T) => [['T, 'T]];
                      """;
        var result = SyntaxUtilities.InspectSyntax(code);
        var doubler = (FuncTypeEntity)result.Context.Types.LookupDereferencedType("ArrayDoubler")!;
        var p1 = (FuncTypeEntity)doubler.ProvideTypeArgument(new TypeArgEntity("'T", null), TypeEntity.Int);
        var tupType = (TupleTypeEntity)p1.ResultTypeEntity;
        Assert.Equal(TypeEntity.Int, tupType.ElementTypeEntities[0]);
        Assert.Equal(TypeEntity.Int, tupType.ElementTypeEntities[1]);
    }
}