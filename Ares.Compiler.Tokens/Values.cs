using Ares.Compiler.Parser;
using Ares.Compiler.Parser.Syntax;
using Ares.Compiler.Transformer;
using Newtonsoft.Json;

namespace Ares.Compiler.Tokens;

public enum ValueTokenType
{
    IntLiteral,
    FloatLiteral,
    BoolLiteral,
    StringLiteral
}

public abstract record ValueToken([JsonProperty(Order = 1)] ValueTokenType ValueType) : SyntaxToken
{

    public static ValueToken Parse(string code) => TokenParser.ParseToken<ValueToken>(code);
    public static explicit operator ValueToken(Value.ValueSyntaxElement syntax) =>
        ValueTransformer.TransformValue(syntax);
}

public record IntLiteralValueToken(int Value) : ValueToken(ValueTokenType.IntLiteral);

public record FloatLiteralValueToken(float Value) : ValueToken(ValueTokenType.FloatLiteral);

public record BoolLiteralValueToken(bool Value) : ValueToken(ValueTokenType.BoolLiteral);

public record StringLiteralValueToken(string Value) : ValueToken(ValueTokenType.StringLiteral);