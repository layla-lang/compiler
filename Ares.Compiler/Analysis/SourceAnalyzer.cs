using System.Collections.Immutable;
using Ares.Compiler.Analysis.Entities;
using Ares.Compiler.Tokens;

namespace Ares.Compiler.Analysis;

public class SourceAnalyzer
{
    private readonly SourceContext context;

    public SourceAnalyzer(SourceContext? priorContext = null)
    {
        context = priorContext ?? new SourceContext();
    }

    public SourceContext Context => context;
    
    public SourceContext AnalyzeContext(ContextToken token)
    {
        var contextContext = context.NewScoped(token.Name);
        return ContextExtensions.AnalyzeContext(token, contextContext);
    }
    public SourceContext AnalyzeStatement(StatementToken token)
    {
        var statementContext = context.NewScoped(token.Tree.Code.SourceName);
        return StatementExtensions.AnalyzeStatement(token, statementContext);
    }
}