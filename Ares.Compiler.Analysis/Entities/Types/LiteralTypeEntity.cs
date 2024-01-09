using Ares.Compiler.Tokens;

namespace Ares.Compiler.Analysis.Entities.Types;

public class LiteralTypeEntity : TypeEntity, IEquatable<LiteralTypeEntity>
{
    public LiteralTypeEntity(string literalString, LiteralKind literalKind, SyntaxToken token) : base(TypeEntityKind.Literal, token)
    {
        this.LiteralString = literalString;
        this.LiteralKind = literalKind;
    }
    
    public string LiteralString { get; init; }
    public LiteralKind LiteralKind { get; init; }
    public override string Name => $"`{Kind}\"{LiteralString}\"";
    public override TypeEntity ProvideTypeArgument(TypeArgEntity tp, TypeEntity v) => this;
    public override bool IsClosed => true;
    #region "Equality"
    
    public bool Equals(LiteralTypeEntity other)
    {
        return LiteralString == other.LiteralString && LiteralKind == other.LiteralKind;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((LiteralTypeEntity)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(base.GetHashCode(), LiteralString, (int)LiteralKind);
    }

    public static bool operator ==(LiteralTypeEntity? left, LiteralTypeEntity? right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(LiteralTypeEntity? left, LiteralTypeEntity? right)
    {
        return !Equals(left, right);
    }

    #endregion
}
