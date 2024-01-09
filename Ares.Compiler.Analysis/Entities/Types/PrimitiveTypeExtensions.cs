namespace Ares.Compiler.Analysis.Entities.Types;

public static class PrimitiveTypeExtensions
{
    public static TypeEntity ToEntity(this PrimitiveTypeKind kind) => kind switch
    {
        PrimitiveTypeKind.Bool => TypeEntity.Bool,
        PrimitiveTypeKind.Byte => TypeEntity.Byte,
        PrimitiveTypeKind.Int => TypeEntity.Int,
        PrimitiveTypeKind.Float => TypeEntity.Float,
        PrimitiveTypeKind.Double => TypeEntity.Double,
        PrimitiveTypeKind.BigNum => TypeEntity.BigNum,
        PrimitiveTypeKind.Char => TypeEntity.Char,
        PrimitiveTypeKind.String => TypeEntity.String,
        _ => throw new ArgumentException($"Unknown primitive type kind {kind}.")
    };

    public static string ToPrimitiveName(this PrimitiveTypeKind kind)
        => Enum.GetName(kind)!;
}