namespace Ares.Runtime.Table.Factory;

internal class NoOpLeveler<TLowLevel> : ILevelChanger<TLowLevel, TLowLevel> where TLowLevel : unmanaged
{
    public TLowLevel Downlevel(TLowLevel highLevel) => highLevel;
    public TLowLevel Uplevel(TLowLevel highLevel) => highLevel;
}