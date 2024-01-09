using System.Runtime.Caching;

namespace Ares.Runtime.Table;

public class CachedLeveler<THighLevel, TLowLevel>
    : ILevelChanger<THighLevel, TLowLevel>
    where TLowLevel : unmanaged
{
    private readonly CacheItemPolicy cacheItemPolicy = new CacheItemPolicy();
    private readonly MemoryCache cache;
    private readonly ILevelChanger<THighLevel, TLowLevel> originalLeveler;

    public CachedLeveler(ILevelChanger<THighLevel, TLowLevel> originalLeveler)
    {
        this.originalLeveler = originalLeveler;
        this.cache = MemoryCache.Default;
    }

    public THighLevel Uplevel(TLowLevel lowLevel) => originalLeveler.Uplevel(lowLevel);

    public TLowLevel Downlevel(THighLevel highLevel)
    {
        var hc = highLevel.GetHashCode().ToString();
        var cacheEntry = cache.Get(hc);
        if (cacheEntry is TLowLevel ll) return ll;
        ll = this.originalLeveler.Downlevel(highLevel);
        cache.Set(new CacheItem(hc, ll), cacheItemPolicy);
        return ll;
    }
}