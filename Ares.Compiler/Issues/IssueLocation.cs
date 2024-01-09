namespace Ares.Compiler.Issues;

public record IssueLocation(string Source, int Line, int Column, long Index);