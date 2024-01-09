namespace Ares.Compiler.Checkpoints;

public class CodeCheckpoint
{
    public int Row { get; private set; }
    public int Col { get; private set; }
    public int Index { get; private set; }
    public CodeCheckpoint(string name)
    {
        this.Name = name;
    }
    
    public string Name { get; init; }
    public int Length => Name.Length;

    public void SetIndex(int index)
    {
        this.Index = index;
    }
    public void SetSourceLocation(int row, int col)
    {
        this.Row = row;
        this.Col = col;
    }

    public override string ToString() => Name;
}