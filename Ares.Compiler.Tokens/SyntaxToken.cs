using System.Text;
using Newtonsoft.Json;

namespace Ares.Compiler.Tokens;

public abstract record SyntaxToken
{
    [JsonIgnore]
    public SyntaxToken? Parent { get; set; }
    public SourceSlice? Slice { get; set; }
    [JsonIgnore]
    public virtual SyntaxTree Tree => Parent!.Tree;
    [JsonIgnore]
    public virtual SourceCode Code => Tree.Code;

    public virtual string SyntaxText => Slice!.Value;

    protected virtual bool PrintMembers(StringBuilder builder)
    {
        builder.Append($"SyntaxText = \"{SyntaxText}\"");
        return true;
    }
}