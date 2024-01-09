using Ares.Compiler.Analysis;
using Ares.Compiler.Checkpoints;
using Ares.Compiler.IO;
using Ares.Compiler.Issues;
using Ares.Compiler.Parser;
using Ares.Compiler.Parser.Syntax;
using Ares.Compiler.Tokens;
using Ares.Compiler.Transformer;
using Newtonsoft.Json;

namespace Ares.Compiler;

public class SyntaxUtilities
{
    public record UtilityResult(List<CompilationIssue> Issues, SyntaxTree Tree, SourceContext Context)
    {
        public bool IsSuccessful => Issues.Count == 0 || Issues.All(i => i.Level < Level.Error);

        public string ToJson()
        {
            var options = new JsonSerializerSettings();
            options.Converters.Add(new SourceContextJsonConverter());
            options.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());
            return JsonConvert.SerializeObject(this, options);
        }
    }

    public static UtilityResult InspectSource(
        CodeCheckpointingInterpolatedStringHandler builder,
        SourceContext? sc = null)
    {
        return InspectSource(builder.GetFormattedText(), sc);
    }
    public static UtilityResult InspectSource(string source, SourceContext? sc = null)
    {
        var contextSyntax = ParserBridge.ParseContext(source);
        var contextToken = ContextTransformer.TransformContext(contextSyntax);
        var sourceCode = new SourceCode("Module", source);
        var result = BuildSyntaxResult(contextSyntax, contextToken, sourceCode);
        var analyzer = new SourceAnalyzer(sc);
        return result with { Context = analyzer.AnalyzeContext(contextToken) };
    }

    public static UtilityResult InspectSyntax(
        CodeCheckpointingInterpolatedStringHandler builder,
        SourceContext? sc = null)
    {
        return InspectSyntax(builder.GetFormattedText(), sc);    
    }
    public static UtilityResult InspectSyntax(string source, SourceContext? sc = null)
    {
        var statementSyntax = ParserBridge.ParseStatement(source);
        var statementToken = StatementTransformer.TransformStatement(statementSyntax);
        var sourceCode = new SourceCode("Module", source);
        var result = BuildSyntaxResult(statementSyntax, statementToken, sourceCode);
        var analyzer = new SourceAnalyzer(sc);
        return result with { Context = analyzer.AnalyzeStatement(statementToken) };
    }

    private static UtilityResult BuildSyntaxResult(Common.SyntaxElement originalElement, SyntaxToken token, SourceCode sourceCode)
    {
        SyntaxTree? tree = null;
        List<CompilationIssue> issues = new List<CompilationIssue>();
        SourceContext context = null;

        try
        {
            tree = new SyntaxTree(sourceCode)
            {
                Token = token,
            };
            token.Slice = SourceSlice.FromSyntaxElementAndCode(originalElement, () => tree.Code);
            token.Parent = tree;
        }
        catch (CodeParser.InternalParserException pex)
        {
            issues.AddRange(pex.ToSyntaxErrors());
        }

        return new UtilityResult(issues, tree, new SourceContext());
    }
}