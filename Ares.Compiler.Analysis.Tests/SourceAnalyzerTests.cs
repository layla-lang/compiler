using Ares.Compiler.Analysis.Entities;
using Ares.Compiler.Analysis.Entities.Types;
using Xunit;

namespace Ares.Compiler.Tests.Analysis;

public class SourceAnalyzerTests
{
    [Fact]
    public void AddsVariablesToTable()
    {
        var result = SyntaxUtilities.InspectSyntax("""var x = 4;""");
        var xType = result.Context.Values["x"].Value.Result.Type;
        Assert.NotNull(xType);
    }

    [Fact]
    public void UsesBuiltinOperators()
    {
        var code = """
                   Int x = 4;
                   Int y = 7;
                   var z1 = x + y;
                   var z2 = x == y;
                   var z3 = x == y ? 3.14 : 5.67;
                   """;
        var result = SyntaxUtilities.InspectSyntax(code);
        var z1Type = result.Context.Values["z1"].Value.Result.Type;
        var z2Type = result.Context.Values["z2"].Value.Result.Type;
        var z3Type = result.Context.Values["z3"].Value.Result.Type;
        Assert.Equal(TypeEntity.Int, z1Type);
        Assert.Equal(TypeEntity.Bool, z2Type);
        Assert.Equal(TypeEntity.Float, z3Type);
    }

    [Fact]
    public void PopulatesMethodTable()
    {
        
        var code = """
                   var complexFunc = (Int x) =>
                        (x * x) % 2 == 0
                            ? 3.14
                            : 1.23;
                   var result = complexFunc(7);
                   """;
        var result = SyntaxUtilities.InspectSyntax(code);
        var resultType = result.Context.Values["result"].Value.Result.Type;
        Assert.Equal(TypeEntity.Float, resultType);
    }

    [Fact]
    public void IdentifiesTypeOfCast()
    {
        var code = """
                   var strCast = (String)3.14;
                   """;
        var result = SyntaxUtilities.InspectSyntax(code);
        var resultType = result.Context.Values["strCast"].Value.Result.Type;
        Assert.Equal(TypeEntity.String, resultType);
    }
    
    [Fact]
    public void MethodInfoAndOperatorFollowsTypeAlias()
    {
        var code = """
                   type Num = Int;
                   var squarer = (Num n) => n * n;
                   var result = squarer(4);
                   """;
        var result = SyntaxUtilities.InspectSyntax(code);
        var resultType = result.Context.Values["result"].Value.Result.Type;
        Assert.Equal(TypeEntity.Int, resultType);
    }

    [Fact]
    public void FollowsMemberAccessOnObject()
    {
        var code = """
                   var x = {
                       age: 24;
                       name: "John";
                   };
                   var y = x.name;
                   """;
        var result = SyntaxUtilities.InspectSyntax(code);
        var resultType = result.Context.Values["y"].Value.Result.Type;
        Assert.Equal(TypeEntity.String, resultType);
    }
    
    [Fact]
    public void TypeInferredFromRecordMember()
    {
        var code = """
                   var user = {
                     age: 29;
                     name: "John";
                   };
                   var name = user.name;
                   """;
        var result = SyntaxUtilities.InspectSyntax(code);
        var userType = result.Context.Values["user"].Value.Result.Type;
        var nameType = result.Context.Values["name"].Value.Result.Type;
        Assert.Equal(new RecordTypeEntity( new Dictionary<string, TypeEntity>
        {
            { "age", TypeEntity.Int },
            { "name", TypeEntity.String }
        }, result.Tree.Token).Name, userType.Name);
        Assert.Equal(TypeEntity.String, nameType);
    }

    [Fact]
    public void WeirdInferenceTest()
    {
        string code = """
                      var x = {
                          age: 24;
                          name: "John";
                      };
                      var y = x;
                      """;
        var result = SyntaxUtilities.InspectSyntax(code);
        var yType = result.Context.Values["y"].Value.Result.Type;
        Assert.Equal(TypeEntityKind.Record, yType.Kind);
    }
    
    [Fact]
    public void CanInferFromTupleExpression()
    {
        string code = """
                      var x = [[3, 1.23, "Hi"]];
                      """;
        var result = SyntaxUtilities.InspectSyntax(code);
        var xType = result.Context.Values["x"].Value.Result.Type;
        Assert.Equal(TypeEntityKind.Tuple, xType.Kind);
    }

    [Fact]
    public void CanInferTupleDestructureStatement()
    {
        string code = """
                      var x = [[1, 3.14, "hi"]];
                      var [[int, float, string]] = x;
                      """;
        var result = SyntaxUtilities.InspectSyntax(code);
        var intType = result.Context.Values["int"].Value.Result.Type;
        var floatType = result.Context.Values["float"].Value.Result.Type;
        var stringType = result.Context.Values["string"].Value.Result.Type;
        Assert.Equal(TypeEntity.Int, intType);
        Assert.Equal(TypeEntity.Float, floatType);
        Assert.Equal(TypeEntity.String, stringType);
    }

    [Fact]
    public void InfersUnionFromDisjointTernaryExpression()
    {
        string code = """
                      var x = 4;
                      var evenOrZero = x % 2 == 0
                        ? "even"
                        : 0;
                      """;
        var result = SyntaxUtilities.InspectSyntax(code);
        var evenOrZeroType = result.Context.Values["evenOrZero"].Value.Result.Type;
        Assert.Equal(TypeEntityKind.Union, evenOrZeroType.Kind);
        var unionType = (UnionTypeEntity)evenOrZeroType;
        Assert.Equal(UnionTypeEntity.CreateTypeUnion(
            new List<TypeEntity>(new []{ TypeEntity.Int, TypeEntity.String })
            , null), unionType);
    }

    [Fact]
    public void CanInferComplicatedNestedTypes()
    {
        var code = """
                   var x = 4;
                   var evenOrZero = x % 2 == 0 ? "even" : 0;
                   var result = {
                       Answer: [evenOrZero];
                   };
                   """;
        var result = SyntaxUtilities.InspectSyntax(code);
        var resultType = result.Context.Values["result"].Value.Result.Type;
        Assert.NotNull(resultType);
    }
}