namespace Ares.Compiler.Tables;

public record CheckpointIndex
{
    public CheckpointIndex(int start, int length)
    {
        Start = start;
        Length = length;
    }
    public int Start { get; }
    public int Length { get; }
}

public interface ICheckpointable
{
    CheckpointIndex Index { get; }
}