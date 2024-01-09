using Ares.Compiler.Tokens;

namespace Ares.Compiler.Analysis.Tables;

public record CheckpointIndex
{
    public CheckpointIndex(int start, int length)
    {
        Start = start;
        Length = length;
    }
    public int Start { get; }
    public int Length { get; }

    public static CheckpointIndex? FromTokenOrNull(SyntaxToken token)
    {
        if (token?.Slice?.StartIndex == null)
        {
            return null;
        }
        return new CheckpointIndex(token.Slice.StartIndex, token.Slice.Length);
    }

}

public interface ICheckpointable
{
    CheckpointIndex Index { get; }
}