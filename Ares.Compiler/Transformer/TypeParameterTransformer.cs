using Ares.Compiler.Parser.Syntax;
using Ares.Compiler.Tokens;

namespace Ares.Compiler.Transformer;

public class TypeParameterTransformer
{
    public static TypeParameterToken TransformTypeParameter(Expression.TypeParameterSyntaxElement tp) => new TypeParameterToken(tp.TypeParameter.Identifier);
}