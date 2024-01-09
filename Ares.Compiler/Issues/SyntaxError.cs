namespace Ares.Compiler.Issues;

public record SyntaxError(string Message, SyntaxError.SyntaxErrorType Type, IssueLocation Location, Level Level = Level.Error) : CompilationIssue(Message, ErrorCode, Location, Level)
{
    public static string ErrorCode = "001";
    
    public enum SyntaxErrorType
    {
        Expected,
        ExpectedString,
        ExpectedCaseInsensitiveString,
        Unexpected,
        UnexpectedString,
        UnexpectedCaseInsensitiveString,
        Message,
        NestedError,
        CompoundError,
        Other,
    }
}