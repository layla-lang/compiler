namespace Ares.Compiler.Tables;

public record Scope(string Name)
{
    public static Scope BuiltIns => new ("_builtins");
}