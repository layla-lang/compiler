using Ares.Compiler.Analysis.Tables;

namespace Ares.Compiler.Checkpoints;

public static class CodeCheckpointExtensions
{
    public static T? GetNearest<T>(this ScopedTable<T> tbl, CodeCheckpoint checkpoint) where T : class, ILookupable, ICheckpointable
    {
        var cpStart = checkpoint.Index;
        var ordered = tbl.ToList()
            .OrderBy(t => t.Index.Start)
            .ToList();
        if (ordered.Count == 0) return null;

        int startIndex = 0;
        int endIndex = ordered.Count;
        int halfIndex = -1;
        T halfItem = null;
        while (endIndex - startIndex > 10)
        {
            halfIndex = startIndex + (endIndex - startIndex) / 2;
            halfItem = ordered[halfIndex]!;
            if (cpStart > halfItem.Index.Start)
            {
                startIndex = halfIndex;
            }
            else
            {
                endIndex = halfIndex;
            }
        }

        var items = ordered.Slice(startIndex, endIndex - startIndex);
        T? lastItem = (startIndex >= 1) ? ordered[startIndex - 1] : null;
        foreach (var i in items)
        {
            if (i.Index.Start > cpStart)
            {
                return lastItem;
            }

            lastItem = i;
        }

        return lastItem;
    }
}