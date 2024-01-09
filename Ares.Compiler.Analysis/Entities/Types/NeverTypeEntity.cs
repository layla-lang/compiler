using Ares.Compiler.Tokens;

namespace Ares.Compiler.Analysis.Entities.Types;

public class NeverTypeEntity(SyntaxToken token) : TypeEntity(TypeEntityKind.Never, token)
{
    public override string Name => "never";
    public override TypeEntity ProvideTypeArgument(TypeArgEntity tp, TypeEntity v) => this;
    public override bool IsClosed => true;
}