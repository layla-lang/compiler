using Ares.Compiler.Analysis.Entities.Types;
using Ares.Runtime.Layout;
using Ares.Runtime.Layout.TypeInformation;
using Ares.Runtime.Table;
using Ares.Runtime.Table.Factory;
using Ares.Runtime.Values;

namespace Ares.Runtime.Memory;

public class Heap
{
    private readonly IPointerTable<TypeEntity> knownTypes;
    private readonly IPointerTable<RuntimeValue> knownValues;
    
    public Heap()
    {
        this.knownTypes = TableFactory.Of<PackedTypeEntity>()
            .WithUniformlySizedItems()
            .WithPinnedMemory()
            .WithLevelingCache()
            .WithLeveler<TypeEntity>(new TypeEntityLeveler())
            .Build();

        var valueLeveler = new RuntimeValueLevel(this.knownTypes);
        
        this.knownValues = TableFactory.Of<PackedValue>()
                .WithUniformlySizedItems()
                .WithPinnedMemory()
                .WithLevelingCache()
                .WithLeveler<RuntimeValue>(valueLeveler)
                .Build(); 
    }

    public IntPtr PileValue(RuntimeValue v) => knownValues.Add(v);

    public RuntimeValue RetrieveValue(IntPtr p) => knownValues[p];
}