using Ares.Compiler.Analysis.Entities.Types;
using Ares.Runtime.Layout;
using Ares.Runtime.Table;
using Ares.Runtime.Values;

namespace Ares.Runtime.Memory;

public class RuntimeValueLevel : ILevelChanger<RuntimeValue, PackedValue>
{
    private readonly IPointerTable<TypeEntity> typeTable;

    public RuntimeValueLevel(IPointerTable<TypeEntity> typeTable)
    {
        this.typeTable = typeTable;
    }
    
    public unsafe PackedValue Downlevel(RuntimeValue v)
    {
        var pv = new PackedValue();
        pv.rtti = typeTable.Add(v.Type);
        var (ptr, size) = v.ToNative();
        pv.data = (byte*)ptr;
        pv.dataSize = size;
        return pv;
    }

    public unsafe RuntimeValue Uplevel(PackedValue lowLevel)
    {
        var te = typeTable[lowLevel.rtti];
        if (te is PrimitiveTypeEntity primitive)
        {
            var ptr = new IntPtr(lowLevel.data);
            var ps = new PointerSpan(ptr, lowLevel.dataSize);
            return primitive.ToValue(ps);
        }
        else
        {
            throw new ArgumentException($"Haven't finished non primitive types yet.");
        }
    }
}