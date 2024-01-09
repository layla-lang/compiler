using Ares.Compiler.Analysis.Entities.Types;
using MessagePack;

namespace Ares.Runtime.Layout.TypeInformation.Packed;

[MessagePackObject]
public class FuncTypeInfoPkg : ITypeInfoPkg
{
    [Key(0)]
    public ITypeInfoPkg[] ParameterTypes { get; set; }
    [Key(1)]
    public ITypeInfoPkg ReturnType { get; set; }
}