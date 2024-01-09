namespace Ares.Runtime.Table;

public abstract record TableCapacity()
{
    public abstract int CalculateTotalBytes(int tSize);
    
    private record ItemCapacity(int items) : TableCapacity
    {
        public override int CalculateTotalBytes(int tSize) => items * tSize;
    }
    private record ByteCapacity(int bytes) : TableCapacity
    {
        public override int CalculateTotalBytes(int _) => bytes;
    }

    public static TableCapacity MaxItems(int items) => new ItemCapacity(items);
    public static TableCapacity MaxBytes(int bytes) => new ByteCapacity(bytes);
}