namespace Ares.Compiler.Tokens;

using System.Collections.Immutable;

public enum IdentifierTokenType
{
    Simple,
    IndexAccess,
    MemberAccess
}

public abstract record IdentifierToken(IdentifierTokenType IdentifierType) : SyntaxToken()
{
}

public record SimpleIdentifierToken(string Text) : IdentifierToken(IdentifierTokenType.Simple);

public record IndexedAccessIdentifierToken(SimpleIdentifierToken Identifier, ExpressionToken AccessExpression)
    : IdentifierToken(IdentifierTokenType.IndexAccess);
public record MemberAccessIdentifierToken(IImmutableList<IdentifierToken> PathIdentifiers, IdentifierToken MemberIdentifier) : IdentifierToken(IdentifierTokenType.MemberAccess);