using System.Collections.Immutable;

namespace Ares.Compiler.Tokens;

public enum TypeConstraintType
{
    IsClosedOver,
    Extends,
}

public abstract record TypeConstraintToken(TypeConstraintType ConstraintType) : SyntaxToken
{
}

public record IsClosedUnderTypeConstraintToken(IImmutableList<string> Operators) : TypeConstraintToken(TypeConstraintType.IsClosedOver);
public record ExtendsConstraintToken(IImmutableList<TypeDescriptorToken> Types) : TypeConstraintToken(TypeConstraintType.Extends);