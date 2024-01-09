namespace Ares.Compiler;

public class SourceFile
{
    public string Text { get; private set; } = "";

    public static SourceFile FromString(string src) => new SourceFile()
    {
        Text = src
    };
    public static SourceFile FromFile(string path) => new SourceFile()
    {
        Text = File.ReadAllText(path)
    };
}