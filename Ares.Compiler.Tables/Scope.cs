namespace Ares.Compiler.Analysis.Tables;

public record Scope(string Name)
{
    public static Scope BuiltIns => new ("_builtins");
}