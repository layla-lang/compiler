using System.Collections.Immutable;
using Ares.Compiler.Tables;
using Ares.Compiler.Tokens;

namespace Ares.Compiler.Analysis.Entities.Types;

public abstract class TypeEntity : ILookupable, ICheckpointable, IEquatable<TypeEntity>
{
    public TypeEntity(TypeEntityKind kind, SyntaxToken token)
    {
        this.Kind = kind;
        this.Token = token;
    }
    public abstract string Name { get; }
    public abstract bool IsClosed { get; }
    public abstract TypeEntity ProvideTypeArgument(TypeArgEntity tp, TypeEntity v);
    public string ResolvedName => (this is IAliased a) ? a.AliasedTo : Name;
    public TypeEntityKind Kind { get; }
    public SyntaxToken Token { get; }
    
    public CheckpointIndex? Index => Token.ToCheckpoint();
    
    public static TypeEntity Any => new AnyTypeEntity(null);
    public static TypeEntity Never => new NeverTypeEntity(null);
    public static TypeEntity Bool => new PrimitiveTypeEntity.BoolTypeEntity();
    public static TypeEntity Byte => new PrimitiveTypeEntity.ByteTypeEntity();
    public static TypeEntity Int => new PrimitiveTypeEntity.IntTypeEntity();
    public static TypeEntity Float => new PrimitiveTypeEntity.FloatTypeEntity();
    public static TypeEntity Double => new PrimitiveTypeEntity.DoubleTypeEntity();
    public static TypeEntity BigNum => new PrimitiveTypeEntity.BigNumTypeEntity();
    public static TypeEntity Char => new PrimitiveTypeEntity.CharTypeEntity();
    public static TypeEntity String => new PrimitiveTypeEntity.StringTypeEntity();

    public static IImmutableDictionary<string, TypeEntity> Primitives => ImmutableList.Create<TypeEntity>()
        .Add(Int)
        .Add(Float)
        .Add(Bool)
        .Add(String)
        .ToImmutableDictionary(kt => kt.Name, kt => kt);
    
    #region "Equality"
    
    public bool Equals(TypeEntity? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Name == other.Name && Kind == other.Kind;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((TypeEntity)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Name, (int)Kind);
    }

    public static bool operator ==(TypeEntity? left, TypeEntity? right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(TypeEntity? left, TypeEntity? right)
    {
        return !Equals(left, right);
    }
    #endregion
}