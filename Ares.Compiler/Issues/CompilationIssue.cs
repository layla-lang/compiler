namespace Ares.Compiler.Issues;

public abstract record CompilationIssue
{
    public CompilationIssue(string message, string code, IssueLocation location, Level level)
    {
        this.Message = message;
        this.Code = code;
        this.Location = location;
        this.Level = level;
    }
    
    public IssueLocation Location { get; init; }
    public Level Level { get; init; }
    public string Code { get; init; }
    public string Message { get; init; }
}