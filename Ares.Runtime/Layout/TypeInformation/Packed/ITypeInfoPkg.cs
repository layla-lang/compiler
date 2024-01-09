namespace Ares.Runtime.Layout.TypeInformation.Packed;

[MessagePack.Union(0, typeof(PrimitiveTypeInfoPkg))]
[MessagePack.Union(1, typeof(ArrayTypeInfoPkg))]
[MessagePack.Union(2, typeof(FuncTypeInfoPkg))]
[MessagePack.Union(3, typeof(PluralTypeInfoPkg))]
[MessagePack.Union(4, typeof(RecordTypeInfoPkg))]
public interface ITypeInfoPkg
{
    
}