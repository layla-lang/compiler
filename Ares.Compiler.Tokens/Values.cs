namespace Ares.Compiler.Tokens;

public enum ValueTokenType
{
    IntLiteral,
    FloatLiteral,
    BoolLiteral,
    StringLiteral
}

public abstract record ValueToken(ValueTokenType ValueType) : SyntaxToken
{
}

public record IntLiteralValueToken(int Value) : ValueToken(ValueTokenType.IntLiteral);

public record FloatLiteralValueToken(float Value) : ValueToken(ValueTokenType.FloatLiteral);

public record BoolLiteralValueToken(bool Value) : ValueToken(ValueTokenType.BoolLiteral);

public record StringLiteralValueToken(string Value) : ValueToken(ValueTokenType.StringLiteral);