using System.Collections.Immutable;
using Ares.Compiler.Tokens;

namespace Ares.Compiler.Analysis.Entities.Types;

public class IntersectionTypeEntity: TypeEntity, ICloseable<IntersectionTypeEntity>, ICloseable, IEquatable<IntersectionTypeEntity>
{
    public IntersectionTypeEntity(IEnumerable<TypeEntity> typeEntities, SyntaxToken token) : base(
        TypeEntityKind.Intersection, token)
    {
        this.TypeEntities = typeEntities.ToImmutableArray();
    }
    
    public IImmutableList<TypeEntity> TypeEntities { get; init; }
    public override string Name => string.Join("&", TypeEntities.Select(te => te.Name));
    public override bool IsClosed => TypeEntities.All(te => te.IsClosed);
        
    IntersectionTypeEntity ICloseable<IntersectionTypeEntity>.ProvideTypeParam(TypeArgEntity tp, TypeEntity v)
        => (IntersectionTypeEntity)ProvideTypeArgument(tp, v);
    
    TypeEntity ICloseable.ProvideTypeParam(TypeArgEntity tp, TypeEntity te)
    {
        var newTypeEntities = TypeEntities.Select(t =>
            t % tp
                ? te
                : t.ProvideTypeArgument(tp, te));
        return new IntersectionTypeEntity(newTypeEntities, Token);
    }
    public override TypeEntity ProvideTypeArgument(TypeArgEntity tp, TypeEntity v) => ((ICloseable)this).ProvideTypeParam(tp, v);

    public static TypeEntity CreateIntersectionType(List<TypeEntity> entities, SyntaxToken token)
    {
        var deduped = entities.Distinct().ToList();
        if (deduped.Count == 1) return deduped.First();

        var resolved = deduped.Select(d => d is AliasedTypeEntity a ? a.ResolveReference() : d).ToList();
        
        if (resolved.All(e => e is RecordTypeEntity))
        {
            var recordTypes = resolved.OfType<RecordTypeEntity>().ToList();
            var members = new Dictionary<string, List<TypeEntity>>();
            foreach (var rc in recordTypes)
            {
                foreach (var m in rc.Members)
                {
                    if (!members.ContainsKey(m.Key))
                    {
                        members.Add(m.Key, new List<TypeEntity>());
                    }
                    members[m.Key].Add(m.Value);
                }
            }

            return new RecordTypeEntity(members.ToDictionary(
                kvp => kvp.Key,
                kvp => UnionTypeEntity.CreateTypeUnion(kvp.Value, null)), null);
        }

        return new IntersectionTypeEntity(deduped, token);
    }
    
    #region "Equality"
    
    public bool Equals(IntersectionTypeEntity other)
    {
        return TypeEntities.SequenceEqual(other.TypeEntities);
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((IntersectionTypeEntity)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(base.GetHashCode(), TypeEntities.GetCombinedHashCode());
    }

    public static bool operator ==(IntersectionTypeEntity? left, IntersectionTypeEntity? right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(IntersectionTypeEntity? left, IntersectionTypeEntity? right)
    {
        return !Equals(left, right);
    }
    #endregion
}