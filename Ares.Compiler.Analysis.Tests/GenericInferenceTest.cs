using Ares.Compiler.Analysis.Entities;
using Ares.Compiler.Analysis.Entities.Types;
using Ares.Compiler.Analysis.Tables;

namespace Ares.Compiler.Tests.Analysis;

public class GenericInferenceTest
{
    [Fact]
    public void GenericTypeDeclarationsCreateScopedTypeEntity()
    {
        string code = """type Arr<'T> = 'T[];""";
        var result = SyntaxUtilities.InspectSyntax(code);
        var nameType = result.Context.Types["Arr"]!.Value.Result;
    
        Assert.IsType<AliasedTypeEntity>(nameType);
        var alias = (AliasedTypeEntity)nameType;
        Assert.Equal(1, alias.Arity);
        Assert.Equal("'T", alias.TypeArgs[0].Identifier);
    }
    
    [Fact]
    public void UnderstandsGenericTypeArgs()
    {
        string code = """
                      type Arr<'T> = 'T[];
                      type IntArr = Arr<Int>;
                      """;
        var result = SyntaxUtilities.InspectSyntax(code);
        var nameType = (AliasedTypeEntity)result.Context.Types.LookupDereferencedType("IntArr");
        Assert.Equal(new ArrayTypeEntity(TypeEntity.Int, null), nameType.ReferencedTypeEntity);
    }
    
    [Fact]
    public void AppliesGenericTypeArgsToRecords()
    {
        string code = """
                      type ResultOf<'T> = {
                          'T[] Result;
                      };
                      type IntResult = ResultOf<Int>;
                      type ResultType = IntResult["Result"] | String;
                      """;
        var result = SyntaxUtilities.InspectSyntax(code);
        var resultType = result.Context.Types.LookupDereferencedType("ResultType");
        Assert.IsType<UnionTypeEntity>(resultType);
        var unionResult = (UnionTypeEntity)resultType;
        var unionArrResult = (ArrayTypeEntity)unionResult.TypeEntities[0];
        var primArrResult = (PrimitiveTypeEntity)unionResult.TypeEntities[1];
        Assert.Equal(TypeEntity.Int, unionArrResult.ElementType);
        Assert.Equal(TypeEntity.String, primArrResult);
    }
}