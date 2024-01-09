using Ares.Compiler.Analysis.Entities.Types;
using Ares.Compiler.Tokens;

namespace Ares.Compiler.Analysis.Helpers;

public static class ReturnTypeInferenceHelper
{
    public static TypeEntity InferReturnType(StatementToken body, SourceContext bodyContext)
    {
        var cs = body.ChildStatementsAndSelf();
        StatementExtensions.AnalyzeStatement(body, bodyContext);
        var returnStmtTypes = cs
            .OfType<ReturnStatementToken>()
            .Select(rt => ExpressionExtensions.TypeOf(rt.Expression, bodyContext))
            .ToList();
        return GeneralizationHelper.Generalize(returnStmtTypes, bodyContext);
    }
}