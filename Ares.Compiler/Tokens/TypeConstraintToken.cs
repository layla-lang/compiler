using System.Collections.Immutable;
using Ares.Compiler.Parser;
using Ares.Compiler.Parser.Syntax;
using Ares.Compiler.Transformer;
using Newtonsoft.Json;

namespace Ares.Compiler.Tokens;

public enum TypeConstraintType
{
    IsClosedOver,
    Extends,
}

public abstract record TypeConstraintToken([JsonProperty(Order = 1)] TypeConstraintType ConstraintType) : SyntaxToken
{
    public static TypeConstraintToken Parse(string code) => TokenParser.ParseToken<TypeConstraintToken>(code);
    public static explicit operator TypeConstraintToken(TypeConstraint.TypeConstraintSyntaxElement syntax) =>
        TypeConstraintTransformer.TransformTypeConstraint(syntax);
}

public record IsClosedUnderTypeConstraintToken(IImmutableList<string> Operators) : TypeConstraintToken(TypeConstraintType.IsClosedOver);
public record ExtendsConstraintToken(IImmutableList<TypeDescriptorToken> Types) : TypeConstraintToken(TypeConstraintType.Extends);