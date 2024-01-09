using Ares.Compiler.Analysis.Entities.Types;
using Ares.Runtime.Layout.TypeInformation;
using Ares.Runtime.Table;

namespace Ares.Runtime.Memory;

public class TypeEntityLeveler : ILevelChanger<TypeEntity, PackedTypeEntity>
{
    public PackedTypeEntity Downlevel(TypeEntity highLevel) => highLevel.ToPacked();

    public TypeEntity Uplevel(PackedTypeEntity lowLevel) => lowLevel.ToUnpacked();
}