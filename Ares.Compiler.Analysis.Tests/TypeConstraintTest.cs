using Ares.Compiler.Analysis.Entities;
using Ares.Compiler.Analysis.Entities.Types;
using Ares.Compiler.Checkpoints;

namespace Ares.Compiler.Tests.Analysis;

public class TypeConstraintTest
{
    [Fact]
    public void WithoutConstraintsOperatorsUnavailableForGenericTypeArgs()
    {
      Assert.Throws<ArgumentException>(() => SyntaxUtilities.InspectSource(
        $$""""
          context Scratchpad;

          public func Add<'T>('T x1, 'T x2) {
            return x1 + x2;
          }
          """"));
    }
    
    [Fact]
    public void IsClosedUnderConstraintEnablesUseOfOperators()
    {
      var _Add = new CodeCheckpoint("Add");
      var result = SyntaxUtilities.InspectSource(
        $$""""
          context Scratchpad;

          given:
            'T is closed under +;
          public func {{_Add}}<'T>('T x1, 'T x2) {
            return x1 + x2;
          }
          """");
      var Add = result.Context.Methods.GetNearest(_Add);
      Assert.Equal(new TypeArgEntity("'T", null), Add.ReturnType);
    }
}