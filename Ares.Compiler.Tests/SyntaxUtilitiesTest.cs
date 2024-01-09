using Ares.Compiler.Issues;
using Ares.Compiler.Tokens;
using Newtonsoft.Json;

namespace Ares.Compiler.Tests;

public class SyntaxUtilitiesTest
{
    [Fact]
    public void ExaminesBlockStatements()
    {
        string src = """
                     x: int = 4
                     """;
        var tree = SyntaxUtilities.InspectSyntax(src);
        Assert.NotNull(tree);
    }

    [Fact]
    public void CreatesCompilationIssues()
    {
        string src = """var x = 4;"""; // Missing semicolon
        var result = SyntaxUtilities.InspectSyntax(src);
        Assert.False(result.IsSuccessful);
        var issue = result.Issues.First();
        Assert.Equal(1, issue.Location.Line);
        Assert.Equal(9, issue.Location.Index);
        Assert.Equal(10, issue.Location.Column);
        Assert.Equal("", issue.Location.Source);
        Assert.Equal(SyntaxError.ErrorCode, issue.Code);
        Assert.Equal("Expected(\"infix operator\")", issue.Message);
    }

    [Fact]
    public void ExtractsSourceSlices()
    {
        string src = """
                     Int x = 4;
                     Int y = 4;
                     """;
        var result = SyntaxUtilities.InspectSyntax(src);
        var token = result.Tree.Token;
        Assert.IsType<BlockStatementToken>(token);
        var blockStmt = (BlockStatementToken)token;
        var stmt1 = blockStmt.Statements[0];
        Assert.IsType<VariableDeclarationStatementToken>(stmt1);
        var varDecl = (VariableDeclarationStatementToken)stmt1;
        Assert.Equal("Int x = 4;", stmt1.Slice.Value);
        Assert.Equal("x", varDecl.Identifier.Slice.Value);
        Assert.Equal("4", varDecl.AssignedValue.Slice.Value);
        
        Assert.NotEmpty(token.Slice.Value);
    }

    [Fact]
    public void SyntaxTreeIsJsonSerializable()
    {
        string src = """
                     x: int = 4
                     """;
        var tree = SyntaxUtilities.InspectSyntax(src);
        var jsonStr = JsonConvert.SerializeObject(tree);
        Assert.NotEmpty(jsonStr);
    }

    [Fact]
    public void FuncTreeIsJsonSerializable()
    {
        string src = """
                      var x = (1, 3.14, "hi");
                      var (int, float, string) = x;
                      """;
        var result = SyntaxUtilities.InspectSyntax(src);
        var jsonStr = result.ToJson();
        Assert.NotEmpty(jsonStr);
    }
}