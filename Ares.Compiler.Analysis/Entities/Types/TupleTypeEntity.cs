using System.Collections.Immutable;
using Ares.Compiler.Tokens;

namespace Ares.Compiler.Analysis.Entities.Types;

public class TupleTypeEntity : TypeEntity, ICloseable<TupleTypeEntity>, ICloseable, IEquatable<TupleTypeEntity>
{
    public TupleTypeEntity(IEnumerable<TypeEntity> elementTypeEntities, SyntaxToken token) : base(TypeEntityKind.Tuple, token)
    {
        this.ElementTypeEntities = elementTypeEntities.ToImmutableArray();
    }
    
    public IImmutableList<TypeEntity> ElementTypeEntities { get; init; }
    public override bool IsClosed => ElementTypeEntities.All(te => te.IsClosed);
    public override string Name => "(" + string.Join(",", ElementTypeEntities.Select(te => te.Name)) + ")";
    
    TupleTypeEntity ICloseable<TupleTypeEntity>.ProvideTypeParam(TypeArgEntity tp, TypeEntity v)
        => (TupleTypeEntity)ProvideTypeArgument(tp, v);
    
    TypeEntity ICloseable.ProvideTypeParam(TypeArgEntity tp, TypeEntity te)
    {
        var newElementTypeEntities = ElementTypeEntities.Select(t =>
            t % tp
                ? te
                : t.ProvideTypeArgument(tp, te));
        return new TupleTypeEntity(newElementTypeEntities, Token);
    }
    public override TypeEntity ProvideTypeArgument(TypeArgEntity tp, TypeEntity v) => ((ICloseable)this).ProvideTypeParam(tp, v);

    #region "Equality"

    public bool Equals(TupleTypeEntity? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return ElementTypeEntities.SequenceEqual(other.ElementTypeEntities);
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((TupleTypeEntity)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(base.GetHashCode(), ElementTypeEntities.GetCombinedHashCode());
    }

    public static bool operator ==(TupleTypeEntity? left, TupleTypeEntity? right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(TupleTypeEntity? left, TupleTypeEntity? right)
    {
        return !Equals(left, right);
    }

    #endregion
}