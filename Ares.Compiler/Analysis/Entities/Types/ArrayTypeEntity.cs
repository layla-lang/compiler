using Ares.Compiler.Tokens;

namespace Ares.Compiler.Analysis.Entities.Types;

public class ArrayTypeEntity : TypeEntity, ICloseable<ArrayTypeEntity>, IEquatable<ArrayTypeEntity>, ICloseable
{
    public ArrayTypeEntity(TypeEntity elementType, SyntaxToken token) : base(TypeEntityKind.Array, token)
    {
        this.ElementType = elementType;
    }
    
    public TypeEntity ElementType { get; init; }
    public override string Name => $"{ElementType.Name}[]";
    public override bool IsClosed => ElementType.IsClosed;

    ArrayTypeEntity ICloseable<ArrayTypeEntity>.ProvideTypeParam(TypeArgEntity tp, TypeEntity v)
        => (ArrayTypeEntity)ProvideTypeArgument(tp, v);
    
    TypeEntity ICloseable.ProvideTypeParam(TypeArgEntity tp, TypeEntity te)
    {
        var newEleType = ElementType % tp
            ? te
            : ElementType.ProvideTypeArgument(tp, te);

        return new ArrayTypeEntity(newEleType, Token);
    }
    public override TypeEntity ProvideTypeArgument(TypeArgEntity tp, TypeEntity v) => ((ICloseable)this).ProvideTypeParam(tp, v);
    
    #region "Equality"
    
    public bool Equals(ArrayTypeEntity other)
    {
        return ElementType.Equals(other.ElementType);
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((ArrayTypeEntity)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(base.GetHashCode(), ElementType);
    }

    public static bool operator ==(ArrayTypeEntity? left, ArrayTypeEntity? right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(ArrayTypeEntity? left, ArrayTypeEntity? right)
    {
        return !Equals(left, right);
    }
    #endregion
}
