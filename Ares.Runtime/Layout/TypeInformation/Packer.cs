using System.Runtime.InteropServices;
using Ares.Compiler.Analysis.Entities.Types;
using Ares.Runtime.Layout.TypeInformation.Packed;
using MessagePack;

namespace Ares.Runtime.Layout.TypeInformation;

public static class Packer
{
    public static unsafe PackedTypeEntity ToPacked(this TypeEntity te)
    {
        var pkg = te.ToPkg();
        var bin = MessagePackSerializer.Serialize(pkg);
        var pte = new PackedTypeEntity();
        fixed (byte* bPtr = bin)
        {
            pte.typeInformation = bPtr;
        }

        pte.typeInfoLength = bin.Length;
        return pte;
    }
    
    public static unsafe TypeEntity ToUnpacked(this PackedTypeEntity pte)
    {
        byte* b = pte.typeInformation;
        var length = pte.typeInfoLength;
        byte[] arr = new byte[length];
        Marshal.Copy((IntPtr)b, arr, 0, length);
        var typeInfoPkg = MessagePackSerializer.Deserialize<ITypeInfoPkg>(arr);
        return typeInfoPkg.ToUnpackedPkg();
    }
    
    private static ITypeInfoPkg ToPkg(this TypeEntity te) => te switch
    {
        PrimitiveTypeEntity primitive => new PrimitiveTypeInfoPkg()
        {
            Kind = primitive.PrimitiveKind
        },
        ArrayTypeEntity array => new ArrayTypeInfoPkg()
        {
            ElementType = array.ElementType.ToPkg()
        },
        TupleTypeEntity tuple => new PluralTypeInfoPkg()
        {
            Kind = PluralTypeKind.Tuple,
            ElementTypes = tuple.ElementTypeEntities
                .Select(te => te.ToPkg())
                .ToArray()
        },
        UnionTypeEntity union => new PluralTypeInfoPkg()
        {
            Kind = PluralTypeKind.Union,
            ElementTypes = union.TypeEntities
                .Select(te => te.ToPkg())
                .ToArray()
        },
        IntersectionTypeEntity intersection => new PluralTypeInfoPkg()
        {
            Kind = PluralTypeKind.Intersection,
            ElementTypes = intersection.TypeEntities
                .Select(te => te.ToPkg())
                .ToArray()
        },
        RecordTypeEntity rec => new RecordTypeInfoPkg()
        {
            Properties = rec.Members.Keys.ToArray(),
            TypeEntities = rec.Members.Values
                .Select(te => te.ToPkg())
                .ToArray()
        },
        FuncTypeEntity fun => new FuncTypeInfoPkg()
        {
            ParameterTypes = fun.ParameterTypeEntities
                .Select(te => te.ToPkg())
                .ToArray(),
            ReturnType = fun.ResultTypeEntity.ToPkg()
        },
        _ => throw new ArgumentException($"Do not know how to pack pointer."),
    };
    
    public static TypeEntity ToUnpackedPkg(this ITypeInfoPkg te) => te switch
    {
        PrimitiveTypeInfoPkg primitive => new PrimitiveTypeEntity(primitive.Kind, null),
        ArrayTypeInfoPkg array => new ArrayTypeEntity(ToUnpackedPkg(array.ElementType), null),
        PluralTypeInfoPkg plural => plural.Kind switch
        {
            PluralTypeKind.Tuple => new TupleTypeEntity(
                plural.ElementTypes.Select(ToUnpackedPkg), null),
            PluralTypeKind.Intersection => new IntersectionTypeEntity(
                plural.ElementTypes.Select(ToUnpackedPkg), null),
            PluralTypeKind.Union => UnionTypeEntity.CreateTypeUnion(
                plural.ElementTypes
                    .Select(ToUnpackedPkg).ToList(), null),
        },
        RecordTypeInfoPkg rec => new RecordTypeEntity(
            Enumerable.Range(0, rec.Properties.Length)
                .ToDictionary(
                    i => rec.Properties[i],
                    i => ToUnpackedPkg(rec.TypeEntities[i])), null),
        FuncTypeInfoPkg fun => new FuncTypeEntity(
            fun.ParameterTypes.Select(ToUnpackedPkg),
            ToUnpackedPkg(fun.ReturnType), null),
        _ => throw new ArgumentException($"Do not know how to pack pointer."),
    };
}