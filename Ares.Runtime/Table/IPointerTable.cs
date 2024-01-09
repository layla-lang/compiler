namespace Ares.Runtime.Table;

public interface IPointerTable<TItem> : IDisposable
{
    IntPtr Add(TItem item);

    TItem this[IntPtr ptr] { get; }
}