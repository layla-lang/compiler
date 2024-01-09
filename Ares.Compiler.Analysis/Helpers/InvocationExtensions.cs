using Ares.Compiler.Analysis.Entities;
using Ares.Compiler.Analysis.Entities.Types;
using Ares.Compiler.Tokens;

namespace Ares.Compiler.Analysis.Helpers;

public static class InvocationExtensions
{
    private static List<TypeEntity> GetTypeArgTypeEntities(this InvocationExpressionToken invocation, SourceContext context) =>
        invocation.TypeArguments
        .Select(ta => ta.GetTypeEntity(context))
        .ToList();
    
    private static List<TypeEntity> GetArgTypeEntities(this InvocationExpressionToken invocation, SourceContext context) =>
        invocation.Arguments
            .Select(ta => ExpressionExtensions.TypeOf(ta, context))
            .ToList();

    public static InvocationHelper.ResolveMethodRequest ToResolveMethodRequest(
        this InvocationExpressionToken invocation, SourceContext context) =>
        new (
            invocation.Identifier.SyntaxText,
            invocation.GetTypeArgTypeEntities(context),
            invocation.GetArgTypeEntities(context));
    
    public static InvocationHelper.TypeArgInferenceRequest ToInferTypeArgsRequest(
        this InvocationExpressionToken invocation, KnownMethod method, SourceContext context) =>
        new (
            method,
            invocation.GetArgTypeEntities(context));
}