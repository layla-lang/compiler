using Ares.Runtime.Table.Factory;

namespace Ares.Runtime.Table;

public class TableFactory<TLowLevel> where TLowLevel : unmanaged
{
    internal bool uniform = false;
    internal bool pinned = false;
    internal bool caching = false;
    internal TableCapacity capacity = TableCapacity.MaxBytes(1_000_000);

    internal TableFactory()
    {
    }

    public TableFactory<TLowLevel> WithUniformlySizedItems()
    {
        this.uniform = true;
        return this;
    }
    
    public TableFactory<TLowLevel> WithPinnedMemory()
    {
        this.pinned = true;
        return this;
    }
    
    public TableFactory<TLowLevel> WithLevelingCache()
    {
        this.caching = true;
        return this;
    }
    
    public TableFactory<THighLevel, TLowLevel> WithLeveler<THighLevel>(ILevelChanger<THighLevel, TLowLevel> leveler)
    {
        return new TableFactory<THighLevel, TLowLevel>()
        {
            uniform = this.uniform,
            pinned = this.pinned,
            caching = this.caching,
            capacity = this.capacity,
            leveler = leveler
        };
    }
    
    public TableFactory<TLowLevel> WithCapacity(TableCapacity capacity)
    {
        this.capacity = capacity;
        return this;
    }

    public unsafe IPointerTable<TLowLevel> Build()
    {
        if (!uniform || !pinned)
        {
            throw new ArgumentException($"No table implementation set for non-uniform or non-GC pinned yet.");
        }

        ILevelChanger<TLowLevel, TLowLevel> fakeChanger = new NoOpLeveler<TLowLevel>();
        return new UniformPinnedLookupTable<TLowLevel, TLowLevel>(
            capacity.CalculateTotalBytes(sizeof(TLowLevel)), fakeChanger);
    }
}