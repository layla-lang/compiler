using Ares.Compiler.Tokens;

namespace Ares.Compiler.Analysis.Entities.Types;

public class TypeArgEntity : TypeEntity, IEquatable<TypeArgEntity>
{
    public TypeArgEntity(string identifier, SyntaxToken token) : base(TypeEntityKind.TypeArg, token)
    {
        this.ParameterName = identifier;
    }
    public string ParameterName { get; init; }
    public override string Name => ParameterName;
    public override TypeEntity ProvideTypeArgument(TypeArgEntity tp, TypeEntity v) => this;

    public static bool operator %(TypeEntity e, TypeArgEntity p)
    {
        if (e is TypeArgEntity p2)
        {
            return p.ParameterName == p2.ParameterName;
        }

        return false;
    }
    
    public override bool IsClosed => false;
    
    #region "Equality"
    
    public bool Equals(TypeArgEntity? other)
    {
        return ParameterName == other!.ParameterName;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((TypeArgEntity)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(base.GetHashCode(), ParameterName);
    }
    
    public static bool operator ==(TypeArgEntity? left, TypeArgEntity? right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(TypeArgEntity? left, TypeArgEntity? right)
    {
        return !Equals(left, right);
    }
    
    #endregion
}