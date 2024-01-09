using System.Collections.Immutable;
using Ares.Compiler.Analysis.Entities;
using Ares.Compiler.Analysis.Entities.Types;
using Ares.Compiler.Tokens;

namespace Ares.Compiler.Analysis;

public static class TypeDescriptorExtensions
{
    
    public static TypeEntity GetTypeEntity(this TypeDescriptorToken td, SourceContext context) => td switch
    {
        AnyTypeDescriptorToken a => new AnyTypeEntity(a),
        NeverTypeDescriptorToken n => new NeverTypeEntity(n),
        LiteralTypeDescriptorToken lit => lit.Value.GetTypeEntityOfLiteral(),
        TypeParamDescriptorToken tp => new TypeArgEntity(tp.Identifier, tp),
        IdTypeDescriptorToken id => LookupNamedTypeIdentifier(id, context),
        ArrayTypeDescriptorToken arr => new ArrayTypeEntity(GetTypeEntity(arr.ElementType, context), arr),
        TupleTypeDescriptorToken tup => new TupleTypeEntity(
            tup.ElementTypes.Select(et => GetTypeEntity(et, context)).ToImmutableList(), tup),
        UnionTypeDescriptorToken un => UnionTypeEntity.CreateTypeUnion(
            un.Types
                .Select(u => u.GetTypeEntity(context))
                .ToList(), un),
        IntersectionTypeDescriptorToken inter => IntersectionTypeEntity.CreateIntersectionType(
            inter.Types
                .Select(i => i.GetTypeEntity(context))
                .ToList(), inter),
        RecordTypeDescriptorToken rec => new RecordTypeEntity(rec.Members.ToDictionary(
            m => m.Key.SyntaxText,
            m => m.Value.GetTypeEntity(context)), rec),
        FuncTypeDescriptorToken fun => new FuncTypeEntity(
            fun.Parameters
                .Select(p => p.GetTypeEntity(context))
                .ToImmutableList(),
            fun.ReturnType.GetTypeEntity(context), fun),
        IndexedTypeDescriptorToken ind => TypeOfIndexedType(ind, context),
        _ => throw new ArgumentException($"Unknown type descriptor token: {td}")
    };

    public static TypeEntity GetGeneralTypeEntityOfLiteral(this ValueToken vt) => vt switch
    {
        BoolLiteralValueToken b => TypeEntity.Bool,
        IntLiteralValueToken i => TypeEntity.Int,
        FloatLiteralValueToken f => TypeEntity.Float,
        StringLiteralValueToken s => TypeEntity.String,
        _ => throw new ArgumentException($"Unknown literal kind: {vt.ValueType}")
    };
    public static TypeEntity GetTypeEntityOfLiteral(this ValueToken vt) => vt switch
    {
        BoolLiteralValueToken b => new LiteralTypeEntity(b.Value.ToString(), LiteralKind.Bool, vt),
        IntLiteralValueToken i => new LiteralTypeEntity(i.Value.ToString(), LiteralKind.Int, vt),
        FloatLiteralValueToken f => new LiteralTypeEntity(f.Value.ToString(), LiteralKind.Float, vt),
        StringLiteralValueToken s => new LiteralTypeEntity(s.Value, LiteralKind.String, vt),
        _ => throw new ArgumentException($"Unknown literal kind: {vt.ValueType}")
    };
    
    private static TypeEntity TypeOfIndexedType(IndexedTypeDescriptorToken ind, SourceContext context)
    {
        TypeEntity indexedType = GetTypeEntity(ind.IndexedType, context);
        TypeEntity indexer = GetTypeEntity(ind.Indexer, context);
        return TypeOfIndexedType(indexedType, indexer, context);
    }
    public static TypeEntity TypeOfIndexedType(TypeEntity indexedType, TypeEntity indexer, SourceContext context)
    {
        return indexedType switch
        {
            AliasedTypeEntity alias => TypeOfIndexedType(alias.ReferencedTypeEntity, indexer, context),
            RecordTypeEntity rec => GetEntityForIndexedRecord(rec, indexer, context),
            UnionTypeEntity union => TypeOfIndexedUnionType(union, indexer, context),
            ArrayTypeEntity arr => TypeOfIndexedArrayType(arr, indexer, context),
            IntersectionTypeEntity intersection => TypeOfIndexedIntersectionType(intersection, indexer, context),
            _ => throw new ArgumentException($"Could not index type {indexedType}")
        };
    }

    private static TypeEntity TypeOfIndexedUnionType(
        UnionTypeEntity indexedType,
        TypeEntity indexer,
        SourceContext context) => UnionTypeEntity.CreateTypeUnion(
                indexedType.TypeEntities.Select(te => TypeOfIndexedType(te, indexer, context)).ToList(),
                null);

    private static TypeEntity TypeOfIndexedArrayType(
        ArrayTypeEntity arrayType,
        TypeEntity indexer,
        SourceContext context)
    {
        if (indexer == TypeEntity.Int)
        {
            return arrayType.ElementType;
        }
        throw new ArgumentException($"Cannot index array type with item of type: {indexer}");
    }

    
    private static TypeEntity TypeOfIndexedIntersectionType(
        IntersectionTypeEntity indexedType,
        TypeEntity indexer,
        SourceContext context) => IntersectionTypeEntity.CreateIntersectionType(
        indexedType.TypeEntities.Select(te => TypeOfIndexedType(te, indexer, context)).ToList(),
        null);
    
    private static TypeEntity GetEntityForIndexedRecord(RecordTypeEntity rec, TypeEntity indexer, SourceContext context)
    {
        if (indexer is LiteralTypeEntity lit)
        {
            if (lit.LiteralKind != LiteralKind.String)
            {
                return new NeverTypeEntity(indexer.Token);
            }

            var str = lit.LiteralString;
            var memberLookup = rec.LookupMember(str);
            return memberLookup ?? new NeverTypeEntity(indexer.Token);
        }
        else if (indexer == TypeEntity.String)
        {
            return UnionTypeEntity.CreateTypeUnion(
                rec.Members
                    .Select(kvp => kvp.Value)
                    .Distinct()
                    .ToList(), indexer.Token);
        }
        else if (indexer is UnionTypeEntity union)
        {
            var uts = union.TypeEntities
                .Select(t => GetEntityForIndexedRecord(rec, t, context))
                .Distinct()
                .ToList();
            return UnionTypeEntity.CreateTypeUnion(uts, indexer.Token);
        }
        return new NeverTypeEntity(indexer.Token);
    }

    private static TypeEntity LookupNamedTypeIdentifier(IdTypeDescriptorToken id, SourceContext context)
    {
        var result = context.Types[id.Identifier.SyntaxText];
        if (!result.HasValue)
        {
            throw new ArgumentException($"Unknown identified type: {id}");
        }

        var te = result.Value.Result;
        if (te is AliasedTypeEntity alias)
        {
            for (var i = 0; i < id.TypeArguments.Count; i++)
            {
                var teArgId = alias.TypeParameters[i];
                var argEntity = id.TypeArguments[i].GetTypeEntity(context);
                var argTe = new TypeArgEntity(teArgId.Identifier, id.TypeArguments[i]);
                te = te.ProvideTypeArgument(argTe, argEntity);
            }
        }
        return te;
    }
}