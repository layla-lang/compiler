using Ares.Compiler.Parser;
using Ares.Compiler.Parser.Syntax;
using Ares.Compiler.Transformer;

namespace Ares.Compiler.Tokens;

public sealed record TypeParameterToken(string Identifier) : SyntaxToken()
{
    public static TypeParameterToken Parse(string code) => TokenParser.ParseToken<TypeParameterToken>(code);
    public static explicit operator TypeParameterToken(Expression.TypeParameterSyntaxElement syntax) =>
        TypeParameterTransformer.TransformTypeParameter(syntax);
}