using System.Runtime.CompilerServices;
using System.Text;

namespace Ares.Compiler.Checkpoints;

[InterpolatedStringHandler]
public ref struct CodeCheckpointingInterpolatedStringHandler
{
    StringBuilder builder;
    private Queue<CodeCheckpoint> checkpoints;

    public CodeCheckpointingInterpolatedStringHandler(int literalLength, int formattedCount)
    {
        builder = new StringBuilder(literalLength);
        checkpoints = new Queue<CodeCheckpoint>();
        Console.WriteLine($"\tliteral length: {literalLength}, formattedCount: {formattedCount}");
    }
    
    public void AppendLiteral(string s)
    {
        builder.Append(s);
    }

    public void AppendFormatted<T>(T t)
    {
        if (t is CodeCheckpoint cp)
        {
            this.AppendCodeCheckpoint(cp);
            return;
        }
        
        builder.Append(t?.ToString());
    }

    private void AppendCodeCheckpoint(CodeCheckpoint checkpoint)
    {
        checkpoint.SetIndex(builder.Length);
        builder.Append(checkpoint.Name);
        this.checkpoints.Enqueue(checkpoint);
    }

    internal string GetFormattedText()
    {
        var result = builder.ToString();
        UpdateCheckpointRowCol(result);
        return result;
    }

    private void UpdateCheckpointRowCol(string result)
    {
        if (checkpoints.Count == 0) return;
        
        var lines = result.Split('\n');
        int index = 0;
        int row = 1;
        CodeCheckpoint cp = checkpoints.Dequeue();

        while (row <= lines.Length)
        {
            var line = lines[row - 1];
            if (index + line.Length + 1 > cp.Index) {
                cp.SetSourceLocation(row, 1 + cp.Index - index);
                if (checkpoints.Count == 0) break;
                cp = checkpoints.Dequeue();
            }

            row++;
            index += line.Length + 1;
        }
        cp.SetSourceLocation(row, cp.Index - index);
    }
}