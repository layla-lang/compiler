using System.Collections.Immutable;
using Ares.Compiler.Tokens;

namespace Ares.Compiler.Analysis.Entities.Types;

public record TypeParam
{
    public TypeParam(string id)
    {
        this.Identifier = id;
        this.Constraints = ImmutableList.Create<TypeConstraintToken>();
    }
    public TypeParam(string id, IImmutableList<TypeConstraintToken> constraints)
    {
        this.Identifier = id;
        this.Constraints = constraints;
    }
    
    public string Identifier { get; init; }
    public IImmutableList<TypeConstraintToken> Constraints { get; init; }
    
};