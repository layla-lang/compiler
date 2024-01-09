using System.Text;

namespace Ares.Compiler.Tokens;

public abstract record SyntaxToken
{
    public SyntaxToken? Parent { get; set; }
    public SourceSlice? Slice { get; set; }
    public virtual SyntaxTree Tree => Parent!.Tree;
    public virtual SourceCode Code => Tree.Code;

    public virtual string SyntaxText => Slice!.Value;

    protected virtual bool PrintMembers(StringBuilder builder)
    {
        builder.Append($"SyntaxText = \"{SyntaxText}\"");
        return true;
    }
}