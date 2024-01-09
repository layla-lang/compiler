using System.Text;

namespace Ares.Compiler.Tokens;

public record SyntaxTree : SyntaxToken
{
    private readonly SourceCode code;

    public SyntaxTree(SourceCode code)
    {
        this.code = code;
    }

    public override SyntaxTree Tree => this;
    public override SourceCode Code => code;
    public override string SyntaxText => Code.Code;
    
    public SyntaxToken? Token { get; set; }

    protected override bool PrintMembers(StringBuilder builder)
    {
        return base.PrintMembers(builder);
    }
}