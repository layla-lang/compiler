using Ares.Compiler.Analysis.Entities;
using Ares.Compiler.Analysis.Entities.Types;

namespace Ares.Compiler.Tests.Analysis.Helpers;

public class FuncKnownMethodBuilderTest
{
    [Fact]
    public void FuncParameterAddsKnownMethodToContext()
    {
        var result = SyntaxUtilities.InspectSource(
            $$"""
              context Scratchpad;

              public type Identity<'T> = ('T) => 'T;
              public func Id<'T>(Identity<'T> identity, 'T val) {
                return identity(val);
              }

              scratch {
                var s = Id((Int x) => x, 4);
              }
              """);
        var s = result.Context.Values["s"]!.Value.Result!;
        Assert.Equal(TypeEntity.String, s.Type);
    }
}