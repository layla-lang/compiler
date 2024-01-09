using System.Collections.Immutable;
using Ares.Compiler.Parser;
using Ares.Compiler.Parser.Syntax;
using Ares.Compiler.Transformer;

namespace Ares.Compiler.Tokens;

public record ContextToken(string Name, IImmutableList<MemberToken> Members) : SyntaxToken
{

    public static StatementToken Parse(string code) => TokenParser.ParseToken<StatementToken>($"code");
    public static explicit operator ContextToken(Context.ContextSyntaxElement syntax) =>
        ContextTransformer.TransformContext(syntax);
}