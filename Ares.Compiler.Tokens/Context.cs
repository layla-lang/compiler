using System.Collections.Immutable;

namespace Ares.Compiler.Tokens;

public record ContextToken(string Name, IImmutableList<MemberToken> Members) : SyntaxToken
{
}