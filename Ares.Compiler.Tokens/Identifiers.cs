using Ares.Compiler.Parser;
using Ares.Compiler.Parser.Syntax;
using Ares.Compiler.Transformer;
using Newtonsoft.Json;

namespace Ares.Compiler.Tokens;

using System.Collections.Immutable;

public enum IdentifierTokenType
{
    Simple,
    IndexAccess,
    MemberAccess
}

public abstract record IdentifierToken([JsonProperty(Order = 1)] IdentifierTokenType IdentifierType) : SyntaxToken()
{
    public static IdentifierToken Parse(string code) => TokenParser.ParseToken<IdentifierToken>(code);
    public static explicit operator IdentifierToken(Expression.IdentifierSyntaxElement syntax) =>
        IdentifierTransformer.TransformIdentifier(syntax);
}

public record SimpleIdentifierToken(string Text) : IdentifierToken(IdentifierTokenType.Simple);

public record IndexedAccessIdentifierToken(SimpleIdentifierToken Identifier, ExpressionToken AccessExpression)
    : IdentifierToken(IdentifierTokenType.IndexAccess);
public record MemberAccessIdentifierToken(IImmutableList<IdentifierToken> PathIdentifiers, IdentifierToken MemberIdentifier) : IdentifierToken(IdentifierTokenType.MemberAccess);