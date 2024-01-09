using System.Collections.Immutable;
using Ares.Compiler.Tokens;

namespace Ares.Compiler.Analysis.Entities.Types;

public class RecordTypeEntity : TypeEntity, ICloseable<RecordTypeEntity>, ICloseable, IEquatable<RecordTypeEntity>
{
    public RecordTypeEntity(Dictionary<string, TypeEntity> members, SyntaxToken token) : base(TypeEntityKind.Record, token)
    {
        this.Members = members.ToImmutableDictionary();
    }
    public IImmutableDictionary<string, TypeEntity> Members { get; init; }
    public TypeEntity? LookupMember(string name) => Members.ContainsKey(name) ? Members[name] : null;
    public override bool IsClosed => Members.Values.All(te => te.IsClosed);
    public override string Name => "{" + string.Join(";", Members
        .OrderBy(m => m.Key)
        .Select(m => $"{m.Key}:{m.Value.Name}")) + ")";
    
    RecordTypeEntity ICloseable<RecordTypeEntity>.ProvideTypeParam(TypeArgEntity tp, TypeEntity v)
        => (RecordTypeEntity)ProvideTypeArgument(tp, v);
    
    TypeEntity ICloseable.ProvideTypeParam(TypeArgEntity tp, TypeEntity te)
    {
        var newMembers = Members.Select(kvp =>
                kvp.Value % tp
                    ? new KeyValuePair<string, TypeEntity>(kvp.Key, te)
                    : new KeyValuePair<string, TypeEntity>(kvp.Key, kvp.Value.ProvideTypeArgument(tp, te)))
            .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        return new RecordTypeEntity(newMembers, Token);
    }
    public override TypeEntity ProvideTypeArgument(TypeArgEntity tp, TypeEntity v) => ((ICloseable)this).ProvideTypeParam(tp, v);

    #region "Equality"

    public bool Equals(RecordTypeEntity? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Members.SequenceEqual(other.Members);
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((RecordTypeEntity)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(base.GetHashCode(), Members.GetCombinedHashCode());
    }

    public static bool operator ==(RecordTypeEntity? left, RecordTypeEntity? right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(RecordTypeEntity? left, RecordTypeEntity? right)
    {
        return !Equals(left, right);
    }
    #endregion
}