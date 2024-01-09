using System.Collections.Immutable;
using Ares.Compiler.Analysis;
using Ares.Compiler.Analysis.Entities;
using Ares.Compiler.Analysis.Entities.Types;
using Ares.Compiler.Tokens;

namespace Ares.Compiler.Tests.Analysis;

public class StatementExtensionsTest
{
    [Fact]
    public void VarDeclarationAddsToValuesTable()
    {
        var stmt = (VariableDeclarationStatementToken)StatementToken.Parse("var x = 4;");
        var analyzer = new SourceAnalyzer();
        var ctx = analyzer.AnalyzeStatement(stmt);

        var xVal = ctx.Values["x"]!.Value.Result;
        Assert.Equal("x", xVal.Name);
        Assert.Equal(TypeEntity.Int, xVal.Type);
    }
    
    [Fact]
    public void VarDeclarationWithFuncValueAddsToValuesAndMethodsTable()
    {
        var stmt = (VariableDeclarationStatementToken)StatementToken.Parse("var squarer = (Int x) => x * x;");
        var analyzer = new SourceAnalyzer();
        var ctx = analyzer.AnalyzeStatement(stmt);

        var squarerVal = ctx.Values["squarer"]!.Value.Result;
        Assert.Equal("squarer", squarerVal.Name);
        Assert.Equal(new FuncTypeEntity((new List<TypeEntity>(new []
        {
            TypeEntity.Int, 
        }).ToImmutableList()), TypeEntity.Int, stmt).Name, squarerVal.Type.Name);
        
        var squarerMethod = ctx.Methods["squarer(Int)"]!.Value.Result;
        Assert.Equal("squarer", squarerMethod.MethodName);
        Assert.Equal(TypeEntity.Int, squarerMethod.ParameterTypes.First());
        Assert.Equal(TypeEntity.Int, squarerMethod.ReturnType);
    }

    [Fact]
    public void TypeDeclarationAddsToTypesTable()
    {
        var stmt = (TypeDeclarationStatementToken)StatementToken.Parse("type Number = Int | Float;");
        var analyzer = new SourceAnalyzer();
        var ctx = analyzer.AnalyzeStatement(stmt);

        var ut = UnionTypeEntity.CreateTypeUnion(
            new List<TypeEntity>(new[] { TypeEntity.Int, TypeEntity.Float }), stmt);
        var numberT = ctx.Types["Number"]!.Value!.Result!;
        Assert.Equal(new AliasedTypeEntity("Number", ut, stmt), numberT);
    }
}