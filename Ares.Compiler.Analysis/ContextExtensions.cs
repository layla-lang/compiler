using Ares.Compiler.Tokens;

namespace Ares.Compiler.Analysis;

public class ContextExtensions
{
    public static SourceContext AnalyzeContext(ContextToken token, SourceContext context)
    {
        foreach (var member in token.Members)
        {
            MemberExtensions.AnalyzeMember(member, context);
        }

        return context;
    }
}