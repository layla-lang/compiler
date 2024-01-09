namespace Ares.Runtime.Table;

public interface ILevelChanger<THighLevel, TLowLevel> where TLowLevel : unmanaged
{
    TLowLevel Downlevel(THighLevel highLevel);
    THighLevel Uplevel(TLowLevel lowLevel);
}