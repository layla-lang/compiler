using Ares.Compiler.Analysis.Entities.Types;
using Ares.Runtime.Memory;
using Ares.Runtime.Values;

namespace Ares.Runtime.Tests.Memory;

public class HeapTests
{
    [Fact]
    public void CanAddAndRetrieveValuesFromHeap()
    {
        var heap = new Heap();
        var ptr = heap.PileValue(new IntRuntimeValue(4));
        var v = heap.RetrieveValue(ptr);
        
        Assert.Equal(TypeEntity.Int, v.Type);
    }
}