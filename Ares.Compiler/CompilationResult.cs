using Ares.Compiler.Issues;

namespace Ares.Compiler;

public class CompilationResult<TOutput>
{
    public bool Successful => Issues.Count == 0 || Issues.All(i => (int)i.Level < (int)Level.Error);
    public TOutput? Result { get; set; }
    public List<CompilationIssue> Issues { get; set; } = new List<CompilationIssue>();
}