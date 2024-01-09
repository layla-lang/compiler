using Ares.Compiler.Analysis.Entities;
using Ares.Compiler.Analysis.Entities.Types;

namespace Ares.Compiler.Tests.Analysis;

public class ScratchContextAnalysisTest
{
  private string Code = """
                        context Scratchpad;

                        public 'T Square<'T>('T x) {
                          return x;
                        }

                        scratch {
                          var x = Square<Int>(3);
                        }
                        """;

  [Fact]
  public void ScratchStaysInContextScope()
  {
    var result = SyntaxUtilities.InspectSource(Code);
    var xVal = result.Context!.Values["x"]!.Value.Result;
    Assert.Equal(TypeEntity.Int, xVal.Type);
  }
}