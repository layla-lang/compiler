namespace Ares.Runtime.Layout;

public unsafe struct PackedValue
{
    public IntPtr rtti;
    public int dataSize;
    public byte* data;
}