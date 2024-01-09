using Ares.Compiler.Analysis.Entities.Types;
using Ares.Compiler.Tables;
using Ares.Compiler.Tokens;

namespace Ares.Compiler.Analysis.Entities;

public record KnownValue : ILookupable, ICheckpointable
{
    public KnownValue(string name, TypeEntity typeEntity, SyntaxToken token)
    {
        this.Name = name;
        this.Type = typeEntity;
        this.Token = token;
        this.Index = token.ToCheckpoint();
    }

    public string Name { get; }
    public TypeEntity Type { get;  }
    public SyntaxToken? Token { get; }
    public CheckpointIndex? Index { get; }
}