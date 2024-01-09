using Ares.Compiler.Analysis;
using Ares.Compiler.IO;

namespace Ares.Compiler.Tests.IO;

public class SourceContextJsonUtilsTest
{
    [Fact]
    public void ConvertsContextToJsonContext()
    {
        var sc = CreateContext();
        var jsonCtx = SourceContextJsonUtils.ToJson(sc);
        
        Assert.NotNull(jsonCtx);
    }

    private SourceContext CreateContext()
    {
        var code = """
                   type Num = Int;
                   var squarer = (Num n) => n * n;
                   var result = squarer(4);
                   """;
        var result = SyntaxUtilities.InspectSyntax(code);
        return result.Context;
    }
}