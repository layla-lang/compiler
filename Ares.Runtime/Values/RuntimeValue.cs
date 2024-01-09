using System.Numerics;
using Ares.Compiler.Analysis.Entities.Types;
using Ares.Runtime.Layout;

namespace Ares.Runtime.Values;

public abstract record RuntimeValue(TypeEntity Type)
{
    public abstract PointerSpan ToNative();
}
public record BoolRuntimeValue(bool Value) : RuntimeValue(TypeEntity.Bool)
{
    public override PointerSpan ToNative() => Value.ToNative();
}
public record ByteRuntimeValue(byte Value) : RuntimeValue(TypeEntity.Byte)
{
    public override PointerSpan ToNative() => Value.ToNative();
}
public record IntRuntimeValue(int Value) : RuntimeValue(TypeEntity.Int)
{
    public override PointerSpan ToNative() => Value.ToNative();
}
public record FloatRuntimeValue(float Value) : RuntimeValue(TypeEntity.Float)
{
    public override PointerSpan ToNative() => Value.ToNative();
}
public record DoubleRuntimeValue(double Value) : RuntimeValue(TypeEntity.Double)
{
    public override PointerSpan ToNative() => Value.ToNative();
}
public record BigIntRuntimeValue(BigInteger Value) : RuntimeValue(TypeEntity.BigNum)
{
    public override PointerSpan ToNative() => Value.ToNative();
}
public record CharRuntimeValue(char Value) : RuntimeValue(TypeEntity.Char)
{
    public override PointerSpan ToNative() => Value.ToNative();
}
