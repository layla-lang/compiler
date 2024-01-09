using Ares.Compiler.Tokens;

namespace Ares.Compiler.Analysis.Entities.Types;

public class PrimitiveTypeEntity : TypeEntity, IEquatable<PrimitiveTypeEntity>
{
    public PrimitiveTypeEntity(string primitiveName, SyntaxToken token) : base(TypeEntityKind.Primitive, token)
    {
        this.PrimitiveName = primitiveName;
    }
    
    public string PrimitiveName { get; init; }
    public override string Name => PrimitiveName;
    public override TypeEntity ProvideTypeArgument(TypeArgEntity tp, TypeEntity v) => this;
    public override bool IsClosed => true;

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