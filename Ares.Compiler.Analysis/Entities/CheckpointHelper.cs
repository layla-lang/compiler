using Ares.Compiler.Tables;
using Ares.Compiler.Tokens;

namespace Ares.Compiler.Analysis.Entities;

public static class CheckpointHelper
{
    public static CheckpointIndex? ToCheckpoint(this SyntaxToken token)
    {
        if (token?.Slice?.StartIndex == null)
        {
            return null;
        }
        return new CheckpointIndex(token.Slice.StartIndex, token.Slice.Length);
    }
}