using MessagePack;

namespace Ares.Runtime.Layout.TypeInformation.Packed;

[MessagePackObject]
public class ArrayTypeInfoPkg : ITypeInfoPkg
{
    [Key(0)]
    public ITypeInfoPkg ElementType { get; set; }
}