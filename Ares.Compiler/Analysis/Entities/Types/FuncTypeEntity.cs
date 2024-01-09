using System.Collections.Immutable;
using Ares.Compiler.Tokens;

namespace Ares.Compiler.Analysis.Entities.Types;

public class FuncTypeEntity : TypeEntity, ICloseable<FuncTypeEntity>, ICloseable, IEquatable<FuncTypeEntity>
{
    public FuncTypeEntity(IEnumerable<TypeEntity> parameterTypeEntities, TypeEntity resultTypeEntity,
        SyntaxToken token) : base(TypeEntityKind.Func, token)
    {
        this.ParameterTypeEntities = parameterTypeEntities.ToImmutableArray();
        this.ResultTypeEntity = resultTypeEntity;
    }
    
    public IImmutableList<TypeEntity> ParameterTypeEntities { get; init; }
    public TypeEntity ResultTypeEntity { get; set; }
    public override bool IsClosed => ParameterTypeEntities.All(te => te.IsClosed) && ResultTypeEntity.IsClosed;
    public override string Name => "(" + string.Join(",", ParameterTypeEntities.Select(p => p.Name)) + "):" + ResultTypeEntity.Name;
    
    FuncTypeEntity ICloseable<FuncTypeEntity>.ProvideTypeParam(TypeArgEntity tp, TypeEntity v)
        => (FuncTypeEntity)ProvideTypeArgument(tp, v);
    
    TypeEntity ICloseable.ProvideTypeParam(TypeArgEntity tp, TypeEntity te)
    {
        var newParTypes = ParameterTypeEntities.Select(t =>
            t % tp
                ? te
                : t.ProvideTypeArgument(tp, te));
        var newResultType = ResultTypeEntity % tp
            ? te
            : ResultTypeEntity.ProvideTypeArgument(tp, te);
        return new FuncTypeEntity(newParTypes, newResultType, Token);
    }
    public override TypeEntity ProvideTypeArgument(TypeArgEntity tp, TypeEntity v) => ((ICloseable)this).ProvideTypeParam(tp, v);

    #region "Equality"

    public bool Equals(FuncTypeEntity? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return ParameterTypeEntities.SequenceEqual(other.ParameterTypeEntities) && ResultTypeEntity.Equals(other.ResultTypeEntity);
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((FuncTypeEntity)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(base.GetHashCode(), ParameterTypeEntities.GetCombinedHashCode(), ResultTypeEntity);
    }

    public static bool operator ==(FuncTypeEntity? left, FuncTypeEntity? right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(FuncTypeEntity? left, FuncTypeEntity? right)
    {
        return !Equals(left, right);
    }
    #endregion
}