using Ares.Compiler.Tokens;

namespace Ares.Compiler.Analysis.Entities.Types;

public class AnyTypeEntity(SyntaxToken token) : TypeEntity(TypeEntityKind.Any, token)
{
    public override string Name => "any";
    public override TypeEntity ProvideTypeArgument(TypeArgEntity tp, TypeEntity v) => this;
    public override bool IsClosed => true;
}