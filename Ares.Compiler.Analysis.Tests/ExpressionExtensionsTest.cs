using Ares.Compiler.Analysis.Entities;
using Ares.Compiler.Analysis.Entities.Types;

namespace Ares.Compiler.Tests.Analysis;

public class ExpressionExtensionsTest
{
    [Fact]
    public void InfersTypeOfArrayAccess()
    {
        var result = SyntaxUtilities.InspectSyntax(
            $$"""
              var s = [1, 2, 3];
              var i = s[0];
              """);
        var iVal = result.Context.Values["i"]!.Value!.Result;
        Assert.Equal(TypeEntity.Int, iVal.Type);
    }
}