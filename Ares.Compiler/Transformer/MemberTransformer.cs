using System.Collections.Immutable;
using Ares.Compiler.Analysis;
using Ares.Compiler.Parser.Syntax;
using Ares.Compiler.Tokens;

namespace Ares.Compiler.Transformer;

public class MemberTransformer
{
    private static IImmutableDictionary<Member.Scope, MemberScope> ScopeLookup = ImmutableDictionary
        .Create<Member.Scope, MemberScope>()
        .Add(Member.Scope.Public, MemberScope.Public)
        .Add(Member.Scope.Protected, MemberScope.Protected)
        .Add(Member.Scope.Internal, MemberScope.Internal)
        .Add(Member.Scope.Private, MemberScope.Private);
    public static MemberToken TransformMember(Member.MemberSyntaxElement ele) => ele.Member.Tag switch
    {
        Member.Member.Tags.Scratch => TransformScratchMember((Member.Member.Scratch)ele.Member),
        Member.Member.Tags.Function => TransformFuncMember(((Member.Member.Function)ele.Member)),
        Member.Member.Tags.TypeDeclaration => TransformTypeDeclMemberToken(((Member.Member.TypeDeclaration)ele.Member)),
        _ => throw new ArgumentException($"Unknown member type: {ele}")
    };
    
    private static ScratchMemberToken TransformScratchMember(Member.Member.Scratch s)
    {
        var bodyToken = StatementTransformer.TransformStatement(s.Body);
        var token = new ScratchMemberToken(bodyToken);
        bodyToken.Parent = token;
        bodyToken.Slice = SourceSlice.FromSyntaxElementAndCode(s.Body, () => token.Code);
        return token;
    }
    
    private static FuncMemberToken TransformFuncMember(Member.Member.Function f)
    {
        var origParameters = f.Signature.Parameters.ToList();
        var origTypeParameters = f.Signature.TypeParameters.ToList();
        var origTypeConstraints = f.Signature.TypeConstraints.ToList();

        var transformedParameters = origParameters
            .ToImmutableDictionary(
                kvp => kvp.Item2,
                kvp => TypeDescriptorTransformer.TransformTypeDescriptor(kvp.Item1));
        
        var transformedTypeParameters = origTypeParameters
            .ToImmutableDictionary(
                tp => tp.TypeParameter.Identifier, TypeParameterTransformer.TransformTypeParameter);


        var transformedConstraints = new Dictionary<string, List<TypeConstraintToken>>();
        var tcLookup = new Dictionary<TypeConstraint.TypeConstraintSyntaxElement, TypeConstraintToken>();
        foreach (var tc in origTypeConstraints)
        {
            var id = tc.Item1;
            var constraint = tc.Item2;
            if (!transformedConstraints.ContainsKey(id))
            {
                transformedConstraints.Add(id, new List<TypeConstraintToken>());
            }

            var transformed = TypeConstraintTransformer.TransformTypeConstraint(constraint);
            transformedConstraints[id].Add(transformed);
            tcLookup.Add(constraint, transformed);
        }

        FunctionReturnTypeToken returnTypeToken;
        if (f.Signature.ReturnType.IsTypeDescriptor)
        {
            var orig = ((Member.FunctionReturnType.TypeDescriptor)f.Signature.ReturnType).Item;
            var transformed = TypeDescriptorTransformer.TransformTypeDescriptor(orig);
            returnTypeToken = new FunctionSpecifiedReturnTypeToken(transformed);
        }
        else
        {
            returnTypeToken = new FunctionInferredReturnTypeToken();
        }
        
        var bodyToken = StatementTransformer.TransformStatement(f.Body);

        var immutableTypeConstraints = transformedConstraints.ToImmutableDictionary(
            kvp => kvp.Key,
            kvp => (IImmutableList<TypeConstraintToken>)kvp.Value.ToImmutableList());
        
        var token = new FuncMemberToken(
            f.Signature.Name,
            ScopeLookup[f.Signature.Scope]!,
            returnTypeToken,
            transformedParameters,
            transformedTypeParameters,
            immutableTypeConstraints,
            bodyToken);

        for (int i = 0; i < transformedParameters.Count; i++)
        {
            var orig = origParameters[i];
            var transformed = transformedParameters[orig.Item2];
            transformed.Parent = token;
            transformed.Slice = SourceSlice.FromSyntaxElementAndCode(orig.Item1, () => token.Code);
        }
        
        for (int i = 0; i < origTypeParameters.Count; i++)
        {
            var orig = origTypeParameters[i];
            var transformed = transformedTypeParameters[orig.TypeParameter.Identifier];
            transformed.Parent = token;
            transformed.Slice = SourceSlice.FromSyntaxElementAndCode(orig, () => token.Code);
        }
        
        foreach (var c in origTypeConstraints)
        {
            var orig = c.Item2;
            var transformed = tcLookup[orig];
            transformed.Parent = token;
            transformed.Slice = SourceSlice.FromSyntaxElementAndCode(orig, () => token.Code);
        }

        if (f.Signature.ReturnType.IsTypeDescriptor)
        {
            var orig = ((Member.FunctionReturnType.TypeDescriptor)f.Signature.ReturnType).Item;
            var transformed = (FunctionSpecifiedReturnTypeToken)returnTypeToken;
            transformed.TypeDescriptor.Parent = token;
            transformed.TypeDescriptor.Slice = SourceSlice.FromSyntaxElementAndCode(orig, () => token.Code);
        }
        
        bodyToken.Parent = token;
        bodyToken.Slice = SourceSlice.FromSyntaxElementAndCode(f.Body, () => token.Code);

        return token;
    }

    private static TypeDeclarationMemberToken TransformTypeDeclMemberToken(Member.Member.TypeDeclaration t)
    {
        var assignedType = TypeDescriptorTransformer.TransformTypeDescriptor(t.TypeDescriptor);
        var origTypeParameters = t.TypeParameters.ToList();
        var transformedTypeParameters = origTypeParameters
            .Select(TypeParameterTransformer.TransformTypeParameter)
            .ToList();

        var token = new TypeDeclarationMemberToken(
            t.Name,
            ScopeLookup[t.Scope]!,
            transformedTypeParameters.ToImmutableArray(),
            assignedType);
        assignedType.Parent = token;
        assignedType.Slice = SourceSlice.FromSyntaxElementAndCode(t.TypeDescriptor, () => token.Code);

        for (int i = 0; i < transformedTypeParameters.Count; i++)
        {
            var orig = origTypeParameters[i];
            var transformed = transformedTypeParameters[i];
            transformed.Parent = token;
            transformed.Slice = SourceSlice.FromSyntaxElementAndCode(orig, () => token.Code);
        }

        return token;
    }
}