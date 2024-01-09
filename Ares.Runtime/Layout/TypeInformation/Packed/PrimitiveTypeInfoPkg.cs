using Ares.Compiler.Analysis.Entities.Types;
using MessagePack;

namespace Ares.Runtime.Layout.TypeInformation.Packed;

[MessagePackObject]
public class PrimitiveTypeInfoPkg : ITypeInfoPkg
{
    [Key(0)]
    public PrimitiveTypeKind Kind { get; set; }
}