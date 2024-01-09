using Xunit;

namespace Ares.Compiler.Checkpoints.Tests;

public class CodeCheckpointingTest
{
    [Fact]
    public void SetsCheckpointInfo()
    {
        var NonGenericSquare = new CodeCheckpoint("Square");
        var GenericSquare = new CodeCheckpoint("Square");
        var result = SyntaxUtilities.InspectSource(
            $$"""
              context InvocationHelperTest;

              public Int {{NonGenericSquare}}(Int x) {
                return x;
              }

              public 'T {{GenericSquare}}<'T>('T x) {
                return x;
              }
              """);
        var methods = result.Context.Methods;

        var nonGenericSquareMethod = methods.GetNearest(NonGenericSquare);
        var genericSquareMethod = methods.GetNearest(GenericSquare);
        
        Assert.Equal(0, nonGenericSquareMethod.TypeParameters.Count);
        Assert.Equal(1, genericSquareMethod.TypeParameters.Count);
    }
}