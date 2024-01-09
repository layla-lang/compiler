using Ares.Compiler.Parser.Syntax;
using Ares.Compiler.Tokens;

namespace Ares.Compiler.Transformer;

public static class ValueTransformer
{
    public static ValueToken TransformValue(Value.ValueSyntaxElement v) => v.Literal.Tag switch
    {
        Value.Literal.Tags.BoolLiteral => new BoolLiteralValueToken(((Value.Literal.BoolLiteral)v.Literal).Item),
        Value.Literal.Tags.StringLiteral => new StringLiteralValueToken(((Value.Literal.StringLiteral)v.Literal).Item),
        Value.Literal.Tags.IntLiteral => new IntLiteralValueToken(((Value.Literal.IntLiteral)v.Literal).Item),
        Value.Literal.Tags.FloatLiteral => new FloatLiteralValueToken((float)((Value.Literal.FloatLiteral)v.Literal)
            .Item),
        _ => throw new ArgumentException($"Unknown value type: {v}")
    };
}