using System.Runtime.InteropServices;

namespace Ares.Runtime.Table;

public unsafe class UniformPinnedLookupTable<THighLevel, TLowLevel>
    : IPointerTable<THighLevel>, IDisposable
    where TLowLevel: unmanaged
{
    private readonly IntPtr data;
    private readonly int tSize = sizeof(TLowLevel);
    private readonly ILevelChanger<THighLevel, TLowLevel> leveler;
    private int index = 0;

    internal UniformPinnedLookupTable(
        int byteCount,
        ILevelChanger<THighLevel, TLowLevel> leveler)
    {
        this.data = Marshal.AllocHGlobal(byteCount);
        this.leveler = leveler;
    }

    public IntPtr Add(THighLevel item)
    {
        var lowerLevel = leveler.Downlevel(item);
        var structPtr = data + (tSize * index);
        Marshal.StructureToPtr(lowerLevel, structPtr, true);
        index++;
        return structPtr;
    }

    public THighLevel this[IntPtr ptr]
    {
        get
        {
            var lowLevel = Marshal.PtrToStructure<TLowLevel>(ptr);
            var highLevel = leveler.Uplevel(lowLevel);
            return highLevel;
        }
    }

    public void Dispose()
    {
        Marshal.FreeHGlobal(data);
    }
}