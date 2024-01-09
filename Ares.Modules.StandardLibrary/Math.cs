using static Ares.Compiler.Analysis.Entities.Types.PrimitiveTypeEntity;

namespace Ares.Modules.StandardLibrary;


public class Math
{
    [return: AresType<IntTypeEntity>]
    public int Add(int x, int y) => x + y;
}