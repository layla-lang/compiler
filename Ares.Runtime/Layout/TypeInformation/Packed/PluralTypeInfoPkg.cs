using MessagePack;

namespace Ares.Runtime.Layout.TypeInformation.Packed;

[MessagePackObject]
public class PluralTypeInfoPkg : ITypeInfoPkg
{
    [Key(0)]
    public PluralTypeKind Kind { get; set; }
    [Key(1)]
    public ITypeInfoPkg[] ElementTypes { get; set; }
}