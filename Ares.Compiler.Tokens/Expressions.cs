using System.Collections.Immutable;
using Ares.Compiler.Parser;
using Ares.Compiler.Parser.Syntax;
using Ares.Compiler.Transformer;
using Newtonsoft.Json;

namespace Ares.Compiler.Tokens;

public enum ExpressionTokenType
{
    Constant,
    Variable,
    Operation,
    Lambda,
    Invocation,
    Ternary,
    Cast,
    IsType,
    Object,
    Tuple,
    Array
}

public abstract record ExpressionToken([JsonProperty(Order = 1)] ExpressionTokenType ExpressionType) : SyntaxToken()
{
    public static ExpressionToken Parse(string code) => TokenParser.ParseToken<ExpressionToken>(code);

    public static explicit operator ExpressionToken(Expression.ExpressionSyntaxElement syntax) =>
        ExpressionTransformer.TransformExpression(syntax);
}

public record ConstantExpressionToken(ValueToken Value) : ExpressionToken(ExpressionTokenType.Constant);

public record VariableExpressionToken(IdentifierToken Identifier) : ExpressionToken(ExpressionTokenType.Variable);
public enum Operator
{
    Division,
    Multiplication,
    Addition,
    Subtraction,
    Power,
    Modulus,
    Gt,
    Gte,
    Lt,
    Lte,
    Eq,
    NotEq,
}

public record OperationExpressionToken(ExpressionToken Left, Operator Operator, ExpressionToken Right)
    : ExpressionToken(ExpressionTokenType.Operation);

public enum LambdaParameterTypeType
{
    Inferred,
    TypeDescriptor
}

public abstract record LambdaParameterTypeToken([JsonProperty(Order = 1)] LambdaParameterTypeType LambdaParameterTypeType);
public record LambdaInferredParameterTypeToken() : LambdaParameterTypeToken(LambdaParameterTypeType.Inferred);
public record SpecifiedLambdaParameterTypeToken(TypeDescriptorToken TypeDescriptor) : LambdaParameterTypeToken(LambdaParameterTypeType.TypeDescriptor);

public record LambdaParameterToken(IdentifierToken Identifier, LambdaParameterTypeToken Type): SyntaxToken();
public record LambdaExpressionToken(IImmutableList<LambdaParameterToken> Parameters, ExpressionToken Expression)
    : ExpressionToken(ExpressionTokenType.Lambda);

public record InvocationExpressionToken(
    IdentifierToken Identifier,
    IImmutableList<TypeDescriptorToken> TypeArguments,
    IImmutableList<ExpressionToken> Arguments)
    : ExpressionToken(ExpressionTokenType.Invocation);

public record TernaryExpressionToken(ExpressionToken Predicate, ExpressionToken IfTrueExpression, ExpressionToken IfFalseExpression)
    : ExpressionToken(ExpressionTokenType.Ternary);

public record CastExpressionToken(TypeDescriptorToken TypeDescriptor, ExpressionToken CastedValue)
    : ExpressionToken(ExpressionTokenType.Cast);
public record IsTypeExpressionToken(ExpressionToken Expression, TypeDescriptorToken CheckingType)
    : ExpressionToken(ExpressionTokenType.IsType);

public record ObjectMemberToken(IdentifierToken Identifier, ExpressionToken Value): SyntaxToken();
public record ObjectExpressionToken(IImmutableList<ObjectMemberToken> Members)
    : ExpressionToken(ExpressionTokenType.Object);

public record TupleExpressionToken(IImmutableList<ExpressionToken> Elements): ExpressionToken(ExpressionTokenType.Tuple);
public record ArrayExpressionToken(IImmutableList<ExpressionToken> Items): ExpressionToken(ExpressionTokenType.Array);