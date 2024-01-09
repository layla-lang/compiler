namespace Ares.Runtime.Table.Factory;

public static class TableFactory
{
    public static TableFactory<TLowLevel> Of<TLowLevel>() where TLowLevel : unmanaged
        => new TableFactory<TLowLevel>();
    public static TableFactory<THighLevel, TLowLevel> Of<TLowLevel, THighLevel>() where TLowLevel : unmanaged
        => new TableFactory<THighLevel, TLowLevel>();
}