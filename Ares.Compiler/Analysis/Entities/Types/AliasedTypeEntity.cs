using System.Collections.Immutable;
using Ares.Compiler.Analysis.Entities.Types.TypeArgs;
using Ares.Compiler.Analysis.Tables;
using Ares.Compiler.Tokens;

namespace Ares.Compiler.Analysis.Entities.Types;



public class AliasedTypeEntity : TypeEntity, IAliased, IEquatable<AliasedTypeEntity>
{
    public AliasedTypeEntity(
        string aliasName,
        TypeEntity typeEntity,
        SyntaxToken token) : this(aliasName, typeEntity, ImmutableArray<TypeParam>.Empty, token)
    {
    }
    public AliasedTypeEntity(
        string aliasName,
        TypeEntity typeEntity,
        IEnumerable<TypeParam> typeParameters,
        SyntaxToken token) : this(
        aliasName, typeEntity, typeParameters,
        typeParameters.Select(tp => (TypeArg)new OpenTypeArg(tp.Identifier)),
        token)
    {
    }
    private AliasedTypeEntity(
        string aliasName,
        TypeEntity typeEntity,
        IEnumerable<TypeParam> typeParameters,
        IEnumerable<TypeArg> typeArgs,
        SyntaxToken token) : base(TypeEntityKind.Aliased, token)
    {
        this.AliasName = aliasName;
        this.ReferencedTypeEntity = typeEntity;
        this.TypeParameters = typeParameters.ToImmutableArray();
        this.TypeArgs = typeArgs.ToImmutableArray();
    }

    public override string Name => AliasName;
    public string AliasName { get; init; }
    public TypeEntity ReferencedTypeEntity { get; init; }
    public IImmutableList<TypeParam> TypeParameters { get; init; }
    public IImmutableList<TypeArg> TypeArgs { get; init; }
    public IEnumerable<OpenTypeArg> OpenTypeArgs => TypeArgs.OfType<OpenTypeArg>();
    public IEnumerable<ClosedTypeArg> ClosedTypeArgs => TypeArgs.OfType<ClosedTypeArg>();
    public string AliasedTo => ReferencedTypeEntity.Name;
    public int Arity => OpenTypeArgs.Count();
    public override bool IsClosed => Arity == 0;
    public TypeEntity ResolveReference()
    {
        TypeEntity alias = this;
        while (alias is AliasedTypeEntity)
        {
            alias = ((AliasedTypeEntity)alias).ReferencedTypeEntity;
        }

        return alias;
    }

    public override TypeEntity ProvideTypeArgument(TypeArgEntity tp, TypeEntity v)
    {
        var refTypeEntity = ReferencedTypeEntity % tp
            ? v
            : ReferencedTypeEntity.ProvideTypeArgument(tp, v);

        var newTypeArgs = TypeArgs.Select(t => t.Identifier == tp.ParameterName
            ? new ClosedTypeArg(t.Identifier, v)
            : t);
        return new AliasedTypeEntity(
            AliasName,
            refTypeEntity,
            TypeParameters,
            newTypeArgs,
            Token);
    }
    
    #region "Equality"
    
    public bool Equals(AliasedTypeEntity? other)
    {
        return AliasName == other!.AliasName && ReferencedTypeEntity.Equals(other.ReferencedTypeEntity) && TypeParameters.SequenceEqual(other.TypeParameters) && TypeArgs.SequenceEqual(other.TypeArgs);
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((AliasedTypeEntity)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(base.GetHashCode(),
            AliasName, ReferencedTypeEntity, TypeParameters.GetCombinedHashCode(), TypeArgs.GetCombinedHashCode());
    }

    public static bool operator ==(AliasedTypeEntity? left, AliasedTypeEntity? right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(AliasedTypeEntity? left, AliasedTypeEntity? right)
    {
        return !Equals(left, right);
    }

    #endregion
}