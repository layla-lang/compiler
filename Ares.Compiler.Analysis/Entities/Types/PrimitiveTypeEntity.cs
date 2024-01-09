using Ares.Compiler.Tokens;

namespace Ares.Compiler.Analysis.Entities.Types;

public class PrimitiveTypeEntity : TypeEntity, IEquatable<PrimitiveTypeEntity>
{
    public PrimitiveTypeEntity(PrimitiveTypeKind primitiveTypeKind, SyntaxToken token) : base(TypeEntityKind.Primitive, token)
    {
        this.PrimitiveKind = primitiveTypeKind;
    }

    public string PrimitiveName => PrimitiveKind.ToPrimitiveName();
    public PrimitiveTypeKind PrimitiveKind { get; init; }
    public override string Name => PrimitiveName;
    public override TypeEntity ProvideTypeArgument(TypeArgEntity tp, TypeEntity v) => this;
    public override bool IsClosed => true;

    
    public class BoolTypeEntity() : PrimitiveTypeEntity(PrimitiveTypeKind.Bool, null);
    public class ByteTypeEntity() : PrimitiveTypeEntity(PrimitiveTypeKind.Byte, null);
    public class IntTypeEntity() : PrimitiveTypeEntity(PrimitiveTypeKind.Int, null);
    public class FloatTypeEntity() : PrimitiveTypeEntity(PrimitiveTypeKind.Float, null);
    public class DoubleTypeEntity() : PrimitiveTypeEntity(PrimitiveTypeKind.Double, null);
    public class BigNumTypeEntity() : PrimitiveTypeEntity(PrimitiveTypeKind.BigNum, null);
    public class CharTypeEntity() : PrimitiveTypeEntity(PrimitiveTypeKind.Char, null);
    public class StringTypeEntity() : PrimitiveTypeEntity(PrimitiveTypeKind.String, null);
    
    #region "Equality"
    
    public bool Equals(PrimitiveTypeEntity other)
    {
        return PrimitiveName == other.PrimitiveName;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((PrimitiveTypeEntity)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(base.GetHashCode(), PrimitiveName);
    }

    public static bool operator ==(PrimitiveTypeEntity? left, PrimitiveTypeEntity? right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(PrimitiveTypeEntity? left, PrimitiveTypeEntity? right)
    {
        return !Equals(left, right);
    }
    
    /*
     *
       public static bool operator ==(PrimitiveTypeEntity? left, PrimitiveTypeEntity? right)
       {
           if (left is null)
           {
               return right is null;
           }

           return left.Equals(right);
       }

       public static bool operator !=(PrimitiveTypeEntity? left, PrimitiveTypeEntity? right)
       {
           if (left is null)
           {
               return right is not null;
           }

           return !left.Equals(right);
       }

     */
    
    #endregion
}