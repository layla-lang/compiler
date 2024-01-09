using Ares.Runtime.Memory;

namespace Ares.Runtime;

public class Instance
{
    private readonly Heap heap;

    public Instance()
    {
        this.heap = new Heap();
    }
}