using System.Numerics;
using System.Runtime.InteropServices;

namespace Ares.Runtime.Layout;

public static class ValueExtensions
{
    public static PointerSpan ToNative(this bool b) => b.ToBytes().ToNative();
    public static PointerSpan ToNative(this byte b) => new byte[] { b }.ToNative();
    public static PointerSpan ToNative(this int i) => BitConverter.GetBytes(i).ToNative();
    public static PointerSpan ToNative(this float i) => BitConverter.GetBytes(i).ToNative();
    public static PointerSpan ToNative(this double i) => BitConverter.GetBytes(i).ToNative();
    public static PointerSpan ToNative(this char i) => BitConverter.GetBytes(i).ToNative();
    public static PointerSpan ToNative(this BigInteger i) => i.ToByteArray().ToNative();

    public static bool FromNativeBool(this PointerSpan span) => span.ToByteArray()[0] == 1;
    public static byte FromNativeByte(this PointerSpan span) => span.ToByteArray()[0];
    public static int FromNativeInt(this PointerSpan span) => BitConverter.ToInt32(span.ToByteArray());
    public static float FromNativeFloat(this PointerSpan span) => BitConverter.ToSingle(span.ToByteArray());
    public static double FromNativeDouble(this PointerSpan span) => BitConverter.ToDouble(span.ToByteArray());
    public static char FromNativeChar(this PointerSpan span) => BitConverter.ToChar(span.ToByteArray());
    public static BigInteger FromNativeBigInt(this PointerSpan span) => new BigInteger(span.ToByteArray());
    
    private static byte[] ToBytes(this bool b) => b ? new byte[] { 1 } : new byte[] { 0 };
    private static unsafe PointerSpan ToNative(this byte[] bdata)
    {
        fixed(byte* byteData = bdata)
        {
            var ptr = new IntPtr(byteData);
            return new PointerSpan(ptr, bdata.Length);
        }
    }

    public static byte[] ToByteArray(this PointerSpan span)
    {
        var (ptr, len) = span;
        byte[] managedArray = new byte[len];
        Marshal.Copy(ptr, managedArray, 0, len);
        return managedArray;
    }
    
}