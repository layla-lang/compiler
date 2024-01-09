using Ares.Runtime.Table.Factory;

namespace Ares.Runtime.Table;

public class TableFactory<THighLevel, TLowLevel> where TLowLevel : unmanaged
{
    internal bool uniform = false;
    internal bool pinned = false;
    internal bool caching = false;
    internal TableCapacity capacity = TableCapacity.MaxBytes(1_000_000);
    internal ILevelChanger<THighLevel, TLowLevel> leveler;

    internal TableFactory()
    {
    }

    public TableFactory<THighLevel, TLowLevel> WithUniformlySizedItems()
    {
        this.uniform = true;
        return this;
    }
    
    public TableFactory<THighLevel, TLowLevel> WithPinnedMemory()
    {
        this.pinned = true;
        return this;
    }
    
    public TableFactory<THighLevel, TLowLevel> WithLevelingCache()
    {
        this.caching = true;
        return this;
    }
    
    public TableFactory<THighLevel, TLowLevel> WithLeveler(ILevelChanger<THighLevel, TLowLevel> leveler)
    {
        this.leveler = leveler;
        return this;
    }
    
    public TableFactory<THighLevel, TLowLevel> WithCapacity(TableCapacity capacity)
    {
        this.capacity = capacity;
        return this;
    }

    public unsafe IPointerTable<THighLevel> Build()
    {
        if (!uniform || !pinned)
        {
            throw new ArgumentException($"No table implementation set for non-uniform or non-GC pinned yet.");
        }

        ILevelChanger<THighLevel, TLowLevel> finalLeveler = this.caching
            ? new CachedLeveler<THighLevel, TLowLevel>(this.leveler)
            : this.leveler;

        return new UniformPinnedLookupTable<THighLevel, TLowLevel>(
            capacity.CalculateTotalBytes(sizeof(TLowLevel)), finalLeveler);
    }
}