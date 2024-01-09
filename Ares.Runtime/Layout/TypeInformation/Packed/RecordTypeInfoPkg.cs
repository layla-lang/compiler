using Ares.Compiler.Analysis.Entities.Types;
using MessagePack;

namespace Ares.Runtime.Layout.TypeInformation.Packed;

[MessagePackObject]
public class RecordTypeInfoPkg : ITypeInfoPkg
{
    [Key(0)]
    public string[] Properties { get; set; }
    [Key(1)]
    public ITypeInfoPkg[] TypeEntities { get; set; }
}