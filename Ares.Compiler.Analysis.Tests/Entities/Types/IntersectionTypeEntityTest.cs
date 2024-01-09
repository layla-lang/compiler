using Ares.Compiler.Analysis.Entities.Types;

namespace Ares.Compiler.Tests.Analysis.Entities.Types;

public class IntersectionTypeEntityTest
{
    
    [Fact]
    public void IntersectionTypeFlattensRecords()
    {
        var r1 = new RecordTypeEntity(new Dictionary<string, TypeEntity>()
        {
            { "age", TypeEntity.Int },
            { "birthday", TypeEntity.Int },
        }, null);
        var r2 = new RecordTypeEntity(new Dictionary<string, TypeEntity>()
        {
            { "name", TypeEntity.String },
            { "birthday", TypeEntity.String },
        }, null);
        var r3 = IntersectionTypeEntity.CreateIntersectionType(
            new List<TypeEntity>(new[] { r1, r2 }), null);
        Assert.IsType<RecordTypeEntity>(r3);
        Assert.Equal(TypeEntity.String, ((RecordTypeEntity)r3).Members["name"]);
        Assert.Equal(TypeEntity.Int, ((RecordTypeEntity)r3).Members["age"]);

        var stringAndInt = UnionTypeEntity.CreateTypeUnion(new List<TypeEntity>(new[]
        {
            TypeEntity.Int, TypeEntity.String
        }), null);
        Assert.Equal(stringAndInt, ((RecordTypeEntity)r3).Members["birthday"]);
    }
}