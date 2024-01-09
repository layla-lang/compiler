using System.Collections.Immutable;
using Ares.Compiler.Tokens;

namespace Ares.Compiler.Analysis.Entities.Types;

public class UnionTypeEntity : TypeEntity, ICloseable<UnionTypeEntity>, ICloseable, IEquatable<UnionTypeEntity>
{
    private UnionTypeEntity(IEnumerable<TypeEntity> typeEntities, SyntaxToken token) : base(TypeEntityKind.Union, token)
    {
        this.TypeEntities = typeEntities.OrderBy(te => te.Name).ToImmutableArray();
    }
    public IImmutableList<TypeEntity> TypeEntities { get; init; }
    public override string Name => string.Join("|", TypeEntities.Select(te => te.Name));

    public static TypeEntity CreateTypeUnion(List<TypeEntity> entities, SyntaxToken token)
    {
        var uniqueTypes = entities.Distinct().ToList();
        if (uniqueTypes.Count == 1) return uniqueTypes[0];

        return new UnionTypeEntity(uniqueTypes, token);
    }
    
    UnionTypeEntity ICloseable<UnionTypeEntity>.ProvideTypeParam(TypeArgEntity tp, TypeEntity v)
        => (UnionTypeEntity)ProvideTypeArgument(tp, v);
    
    TypeEntity ICloseable.ProvideTypeParam(TypeArgEntity tp, TypeEntity te)
    {
        var newTypeEntities = TypeEntities.Select(t =>
            t % tp
                ? te
                : t.ProvideTypeArgument(tp, te));
        return new UnionTypeEntity(newTypeEntities, Token);
    }
    public override TypeEntity ProvideTypeArgument(TypeArgEntity tp, TypeEntity v) => ((ICloseable)this).ProvideTypeParam(tp, v);
    public override bool IsClosed => TypeEntities.All(te => te.IsClosed);
    #region "Equality"
    
    public bool Equals(UnionTypeEntity? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return TypeEntities.SequenceEqual(other.TypeEntities);
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((UnionTypeEntity)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(base.GetHashCode(), TypeEntities.GetCombinedHashCode());
    }

    public static bool operator ==(UnionTypeEntity? left, UnionTypeEntity? right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(UnionTypeEntity? left, UnionTypeEntity? right)
    {
        return !Equals(left, right);
    }

    #endregion
}