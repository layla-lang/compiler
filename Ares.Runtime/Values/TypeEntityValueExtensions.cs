using System.Collections.Immutable;
using System.Numerics;
using Ares.Compiler.Analysis.Entities.Types;
using Ares.Runtime.Layout;

namespace Ares.Runtime.Values;

public static class TypeEntityValueExtensions
{
    public static RuntimeValue ToValue(this PrimitiveTypeEntity te, PointerSpan ps) => te.PrimitiveKind switch
    {
        PrimitiveTypeKind.Bool => te.ToRuntimeVal(ps.FromNativeBool()),
        PrimitiveTypeKind.Byte => te.ToRuntimeVal(ps.FromNativeByte()),
        PrimitiveTypeKind.Int => te.ToRuntimeVal(ps.FromNativeInt()),
        PrimitiveTypeKind.Float => te.ToRuntimeVal(ps.FromNativeFloat()),
        PrimitiveTypeKind.Double => te.ToRuntimeVal(ps.FromNativeDouble()),
        PrimitiveTypeKind.BigNum => te.ToRuntimeVal(ps.FromNativeBigInt()),
        PrimitiveTypeKind.Char => te.ToRuntimeVal(ps.FromNativeChar()),
        _ => throw new ArgumentException($"Unknown primitive type {te.PrimitiveKind}.")
    };
    
    public static RuntimeValue ToRuntimeVal(this PrimitiveTypeEntity span, bool b) => new BoolRuntimeValue(b);
    public static RuntimeValue ToRuntimeVal(this PrimitiveTypeEntity span, byte b) => new ByteRuntimeValue(b);
    public static RuntimeValue ToRuntimeVal(this PrimitiveTypeEntity span, int b) => new IntRuntimeValue(b);
    public static RuntimeValue ToRuntimeVal(this PrimitiveTypeEntity span, float b) => new FloatRuntimeValue(b);
    public static RuntimeValue ToRuntimeVal(this PrimitiveTypeEntity span, double b) => new DoubleRuntimeValue(b);
    public static RuntimeValue ToRuntimeVal(this PrimitiveTypeEntity span, char b) => new CharRuntimeValue(b);
    public static RuntimeValue ToRuntimeVal(this PrimitiveTypeEntity span, BigInteger b) => new BigIntRuntimeValue(b);

}