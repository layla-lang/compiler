using Ares.Compiler.Analysis.Entities;
using Ares.Compiler.Analysis.Entities.Types;
using Ares.Compiler.Analysis.Tables;

namespace Ares.Compiler.Tests.Analysis;

public class IndexedTypeInferrenceTest
{
    [Fact]
    public void IndexingRecordByNameReturnsMemberType()
    {
        string code = """
                      type R = {
                        Int age;
                        String name;
                        Bool legalAdult;
                      };
                      type NameType = R["name"];
                      """;
        var result = SyntaxUtilities.InspectSyntax(code);
        var nameType = result.Context.Types.LookupDereferencedType("NameType");
        Assert.Equal(TypeEntity.String, nameType);
    }
    
    [Fact]
    public void IndexingRecordByMultipleNamesReturnsUnionType()
    {
        string code = """
                      type R = {
                        Int age;
                        String name;
                        Bool legalAdult;
                      };
                      type NameType = R["name" | "age"];
                      """;
        var result = SyntaxUtilities.InspectSyntax(code);
        var nameType = result.Context.Types.LookupDereferencedType("NameType");
        Assert.IsType<UnionTypeEntity>(nameType);
        var union = (UnionTypeEntity)nameType;
        Assert.Equal(UnionTypeEntity.CreateTypeUnion(
            new List<TypeEntity>(new [] { TypeEntity.String, TypeEntity.Int }), null), union);
    }
    
    [Fact]
    public void IndexingRecordByKeyTypeReturnsUnionOverAllMembers()
    {
        string code = """
                      type R = {
                        Int age;
                        String name;
                        Bool legalAdult;
                      };
                      type NameType = R[String];
                      """;
        var result = SyntaxUtilities.InspectSyntax(code);
        var nameType = result.Context.Types.LookupDereferencedType("NameType");
        Assert.IsType<UnionTypeEntity>(nameType);
        var union = (UnionTypeEntity)nameType;
        Assert.Equal(new TypeEntity[]
        {
            TypeEntity.Bool, 
            TypeEntity.Int, 
            TypeEntity.String, 
        }, union.TypeEntities.OrderBy(te => te.Name).ToArray());
    }
    
    [Fact]
    public void IndexingRecordsResultingInUnionsGetDeduplicated()
    {
        string code = """
                      type R = {
                        String x;
                        String y;
                        String z;
                      };
                      type NameType = R[String];
                      """;
        var result = SyntaxUtilities.InspectSyntax(code);
        var nameType = result.Context.Types.LookupDereferencedType("NameType");
        Assert.Equal(TypeEntity.String, nameType);
    }
    
    [Fact]
    public void IndexingRecordsByOtherTypeResultsInNever()
    {
        string code = """
                      type R = {
                        String x;
                        String y;
                        String z;
                      };
                      type NameType = R[Int];
                      """;
        var result = SyntaxUtilities.InspectSyntax(code);
        var nameType = result.Context.Types.LookupDereferencedType("NameType");
        Assert.Equal(TypeEntity.Never, nameType);
    }
}