using System.Collections.Immutable;
using Ares.Compiler.Parser.Syntax;
using Ares.Compiler.Tokens;

namespace Ares.Compiler.Transformer;

public static class TypeDescriptorTransformer
{
    public static TypeDescriptorToken TransformTypeDescriptor(Expression.TypeDescriptorSyntaxElement td) => td.TypeDescriptor.Tag switch
    {
        Expression.TypeDescriptor.Tags.Any => new AnyTypeDescriptorToken(), 
        Expression.TypeDescriptor.Tags.Never => new NeverTypeDescriptorToken(), 
        Expression.TypeDescriptor.Tags.Identified => TransformIdTypeDescriptor((Expression.TypeDescriptor.Identified)td.TypeDescriptor), 
        Expression.TypeDescriptor.Tags.Literal => TransformLiteralTypeDescriptor((Expression.TypeDescriptor.Literal)td.TypeDescriptor), 
        Expression.TypeDescriptor.Tags.Array => TransformArrayTypeDescriptor((Expression.TypeDescriptor.Array)td.TypeDescriptor),
        Expression.TypeDescriptor.Tags.Tuple => TransformTupleTypeDescriptor((Expression.TypeDescriptor.Tuple)td.TypeDescriptor),
        Expression.TypeDescriptor.Tags.Intersection => TransformIntersectionTypeDescriptor((Expression.TypeDescriptor.Intersection)td.TypeDescriptor),
        Expression.TypeDescriptor.Tags.Union => TransformUnionTypeDescriptor((Expression.TypeDescriptor.Union)td.TypeDescriptor),
        Expression.TypeDescriptor.Tags.Func => TransformFuncTypeDescriptor((Expression.TypeDescriptor.Func)td.TypeDescriptor),
        Expression.TypeDescriptor.Tags.Record => TransformRecordRecordTypeDescriptor((Expression.TypeDescriptor.Record)td.TypeDescriptor),
        Expression.TypeDescriptor.Tags.Indexed => TransformIndexedTypeDescriptor((Expression.TypeDescriptor.Indexed)td.TypeDescriptor),
        Expression.TypeDescriptor.Tags.TypeArgument => TransformTypeParamTypeDescriptor((Expression.TypeDescriptor.TypeArgument)td.TypeDescriptor),
        _ => throw new ArgumentException($"Unknown type descriptor type: {td}")
    };

    private static IdTypeDescriptorToken TransformIdTypeDescriptor(Expression.TypeDescriptor.Identified idTd)
    {
        var id = IdentifierTransformer.TransformIdentifier(idTd.Identifier);
        var originalTypeArgs = idTd.TypeArguments.ToList();
        var transformedTypeArgs = originalTypeArgs
            .Select(TransformTypeDescriptor)
            .ToImmutableList();
        var token = new IdTypeDescriptorToken(id, transformedTypeArgs);
        id.Parent = token;
        id.Slice = SourceSlice.FromSyntaxElementAndCode(idTd.Identifier, () => token.Code);
        for (var i = 0; i < originalTypeArgs.Count; i++)
        {
            var orig = originalTypeArgs[i];
            var transformed = transformedTypeArgs[i];
            transformed.Parent = token;
            transformed.Slice = SourceSlice.FromSyntaxElementAndCode(orig, () => token.Code);
        }
        return token;
    }
    
    private static LiteralTypeDescriptorToken TransformLiteralTypeDescriptor(Expression.TypeDescriptor.Literal litTd)
    {
        var vt = ValueTransformer.TransformValue(litTd.Item);
        var token = new LiteralTypeDescriptorToken(vt);
        vt.Parent = token;
        vt.Slice = SourceSlice.FromSyntaxElementAndCode(litTd.Item, () => token.Code);
        return token;
    }
    
    private static ArrayTypeDescriptorToken TransformArrayTypeDescriptor(Expression.TypeDescriptor.Array arTd)
    {
        var id = TypeDescriptorTransformer.TransformTypeDescriptor(arTd.Item);
        var token = new ArrayTypeDescriptorToken(id);
        id.Parent = token;
        id.Slice = SourceSlice.FromSyntaxElementAndCode(arTd.Item, () => token.Code);
        return token;
    }
    
    private static TupleTypeDescriptorToken TransformTupleTypeDescriptor(Expression.TypeDescriptor.Tuple tupTd)
    {
        var types = ImmutableList.Create<TypeDescriptorToken>()
            .AddRange(tupTd.Item.Select(TransformTypeDescriptor).ToList());
        var token = new TupleTypeDescriptorToken(types);
        for (int i = 0; i < types.Count; i++)
        {
            var o = tupTd.Item[i];
            var t = types[i];
            t.Parent = token;
            t.Slice = SourceSlice.FromSyntaxElementAndCode(o, () => token.Code);
        }

        return token;
    }
    
    private static IntersectionTypeDescriptorToken TransformIntersectionTypeDescriptor(Expression.TypeDescriptor.Intersection iTd)
    {
        var types = ImmutableList.Create<TypeDescriptorToken>()
            .AddRange(iTd.Item.Select(TransformTypeDescriptor).ToList());
        var token = new IntersectionTypeDescriptorToken(types);
        for (int i = 0; i < types.Count; i++)
        {
            var o = iTd.Item[i];
            var t = types[i];
            t.Parent = token;
            t.Slice = SourceSlice.FromSyntaxElementAndCode(o, () => token.Code);
        }

        return token;
    }
    
    private static UnionTypeDescriptorToken TransformUnionTypeDescriptor(Expression.TypeDescriptor.Union uTd)
    {
        var types = ImmutableList.Create<TypeDescriptorToken>()
            .AddRange(uTd.Item.Select(TransformTypeDescriptor).ToList());
        var token = new UnionTypeDescriptorToken(types);
        for (int i = 0; i < types.Count; i++)
        {
            var o = uTd.Item[i];
            var t = types[i];
            t.Parent = token;
            t.Slice = SourceSlice.FromSyntaxElementAndCode(o, () => token.Code);
        }

        return token;
    }

    public static RecordTypeDescriptorToken TransformRecordRecordTypeDescriptor(Expression.TypeDescriptor.Record td)
    {
        var membersMap = td.Item;
        var oldIdToNewIdMap = new Dictionary<Expression.IdentifierSyntaxElement, IdentifierToken>();
        var newMembersMap = new Dictionary<IdentifierToken, TypeDescriptorToken>();
        foreach (var mem in membersMap)
        {
            var oldId = mem.Identifier;
            var oldVal = mem.Type;
            var transformedId = IdentifierTransformer.TransformIdentifier(oldId);
            var transformedVal = TransformTypeDescriptor(oldVal);
            oldIdToNewIdMap.Add(oldId, transformedId);
            newMembersMap.Add(transformedId, transformedVal);
        }
        var immutableDict = ImmutableDictionary.Create<IdentifierToken, TypeDescriptorToken>()
            .AddRange(newMembersMap);
        var token = new RecordTypeDescriptorToken(immutableDict);
        foreach (var mem in membersMap)
        {
            var oldId = mem.Identifier;
            var oldVal = mem.Type;
            var newId = oldIdToNewIdMap[oldId];
            var newVal = newMembersMap[newId];

            newId.Parent = token;
            newId.Slice = SourceSlice.FromSyntaxElementAndCode(oldId, () => token.Code);
            newVal.Parent = token;
            newVal.Slice = SourceSlice.FromSyntaxElementAndCode(oldVal, () => token.Code);
        }

        return token;
    }

    public static FuncTypeDescriptorToken TransformFuncTypeDescriptor(Expression.TypeDescriptor.Func f)
    {
        var typeArgs = ImmutableList.Create<TypeParameterToken>()
            .AddRange(f.TypeParameters.Select(TypeParameterTransformer.TransformTypeParameter));
        var parameters = ImmutableList.Create<TypeDescriptorToken>()
            .AddRange(f.Parameters.Select(TransformTypeDescriptor));
        var returnType = TransformTypeDescriptor(f.ReturnType);
        var token = new FuncTypeDescriptorToken(typeArgs, parameters, returnType);

        for (var i = 0; i < typeArgs.Count; i++)
        {
            var origTa = f.TypeParameters[i];
            var transformedTa = typeArgs[i];
            transformedTa.Parent = token;
            transformedTa.Slice = SourceSlice.FromSyntaxElementAndCode(origTa, () => token.Code);
        }
        
        for (var i = 0; i < f.Parameters.Length; i++)
        {
            var par = f.Parameters[i];
            var td = parameters[i];
            td.Parent = token;
            td.Slice = SourceSlice.FromSyntaxElementAndCode(par, () => token.Code);
        }

        returnType.Parent = token;
        returnType.Slice = SourceSlice.FromSyntaxElementAndCode(f.ReturnType, () => token.Code);
        return token;
    }
    
    public static IndexedTypeDescriptorToken TransformIndexedTypeDescriptor(Expression.TypeDescriptor.Indexed ind)
    {
        var itToken = TransformTypeDescriptor(ind.IndexedType);
        var indToken = TransformTypeDescriptor(ind.Indexer);
        var token = new IndexedTypeDescriptorToken(itToken, indToken);
        itToken.Parent = token;
        itToken.Slice = SourceSlice.FromSyntaxElementAndCode(ind.IndexedType, () => token.Code);
        indToken.Parent = token;
        indToken.Slice = SourceSlice.FromSyntaxElementAndCode(ind.Indexer, () => token.Code);
        return token;
    }

    public static TypeParamDescriptorToken TransformTypeParamTypeDescriptor(Expression.TypeDescriptor.TypeArgument tp) =>
        new TypeParamDescriptorToken(tp.Item);
}