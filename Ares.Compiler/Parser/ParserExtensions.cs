using Ares.Compiler.Parser.Syntax;
using Ares.Compiler.Tokens;
using Ares.Compiler.Transformer;

namespace Ares.Compiler.Parser;

public static class ParserExtensions
{
    public static ContextToken AsToken(this Context.ContextSyntaxElement element) =>
        ContextTransformer.TransformContext(element);
    public static TypeConstraintToken AsToken(this TypeConstraint.TypeConstraintSyntaxElement element) =>
        TypeConstraintTransformer.TransformTypeConstraint(element);
    public static ExpressionToken AsToken(this Expression.ExpressionSyntaxElement element) =>
        ExpressionTransformer.TransformExpression(element);
    public static TypeParameterToken AsToken(this Expression.TypeParameterSyntaxElement element) =>
        TypeParameterTransformer.TransformTypeParameter(element);
    public static TypeDescriptorToken AsToken(this Expression.TypeDescriptorSyntaxElement element) =>
        TypeDescriptorTransformer.TransformTypeDescriptor(element);
    public static StatementToken AsToken(this Statement.StatementSyntaxElement element) =>
        StatementTransformer.TransformStatement(element);
    public static MemberToken AsToken(this Member.MemberSyntaxElement element) =>
        MemberTransformer.TransformMember(element);
}