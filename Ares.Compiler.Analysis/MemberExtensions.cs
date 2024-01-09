using System.Collections.Immutable;
using Ares.Compiler.Analysis.Entities;
using Ares.Compiler.Analysis.Entities.Types;
using Ares.Compiler.Analysis.Helpers;
using Ares.Compiler.Tokens;

namespace Ares.Compiler.Analysis;

public class MemberExtensions
{
    public static SourceContext AnalyzeMember(MemberToken token, SourceContext context)
    {
        switch (token.MemberType)
        {
            case MemberTokenType.Func:
                AnalyzeFuncMember((FuncMemberToken)token, context);
                break;
            case MemberTokenType.Scratch:
                AnalyzeScratchMember((ScratchMemberToken)token, context);
                break;
            case MemberTokenType.TypeDeclaration:
                AnalyzeTypeDeclarationMember((TypeDeclarationMemberToken)token, context);
                break;
        }

        return context;
    }

    private static SourceContext AnalyzeScratchMember(ScratchMemberToken s, SourceContext context)
    {
        return StatementExtensions.AnalyzeStatement(s.Body, context);
    }

    private static SourceContext AnalyzeTypeDeclarationMember(TypeDeclarationMemberToken f, SourceContext context)
    {
        var name = f.Name;
        var genericContext = context.NewScoped(name);
        foreach (var tp in f.TypeParameters)
        {
            genericContext.Types.Add(new TypeArgEntity(tp.Identifier, tp));
        }

        var tps = f.TypeParameters
            .Select(tp => new TypeParam(tp.Identifier))
            .ToImmutableList();
        context.Types.Add(new AliasedTypeEntity(
            name,
            f.AssignedType.GetTypeEntity(genericContext),
            tps,
            f));
        return context;
    }

    private static SourceContext AnalyzeFuncMember(FuncMemberToken f, SourceContext context)
    {
        var (args, typeParams, bodyContext) = FuncKnownMethodBuilder.PartiallyBuild(f, context);
        
        TypeEntity returnType;
        if (f.ReturnType is FunctionSpecifiedReturnTypeToken fsrt)
        {
            returnType = fsrt.TypeDescriptor.GetTypeEntity(context);
        }
        else
        {
            returnType = ReturnTypeInferenceHelper.InferReturnType(f.Body, bodyContext);
        }
        var km = new KnownMethod(
            f.Name,
            args.Values.ToImmutableList(),
            typeParams.ToImmutableList(),
            returnType,
            bodyContext,
            f);
        context.Methods.Add(km);
        return context;
    }
}