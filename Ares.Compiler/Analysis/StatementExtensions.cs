using System.Collections.Immutable;
using Ares.Compiler.Analysis.Entities;
using Ares.Compiler.Analysis.Entities.Types;
using Ares.Compiler.Tokens;

namespace Ares.Compiler.Analysis;

public static class StatementExtensions
{
    public static SourceContext AnalyzeStatement(StatementToken token, SourceContext context)
    {
        switch (token.StatementType)
        {
            case StatementTokenType.Block:
                var blk = (BlockStatementToken)token;
                foreach (var stmt in blk.Statements)
                {
                    AnalyzeStatement(stmt, context);
                }
                break;
            case StatementTokenType.VariableDeclaration:
                AnalyzeVarDeclarationStatement((VariableDeclarationStatementToken)token, context);
                break;
            case StatementTokenType.DestructuredVariableDeclaration:
                AnalyzeDestructuredDeclStatement((DestructuredVariableDeclarationStatementToken)token, context);
                break;
            case StatementTokenType.TypeDeclaration:
                AnalyzeTypeDeclarationStatement((TypeDeclarationStatementToken)token, context);
                break;
            case StatementTokenType.Return:
                break;
            default:
                throw new ArgumentException($"Unknown statement type.");
        }

        return context;
    }
    
    private static void AnalyzeVarDeclarationStatement(VariableDeclarationStatementToken varDecl, SourceContext context)
    {
        TypeEntity varType;
        if (varDecl.DeclaredType.DeclarationTypeType == DeclarationTypeType.TypeDescriptor)
        {
            TypeDescriptorToken td = ((TypeDescriptorTypeToken)varDecl.DeclaredType).TypeDescriptor;
            varType = td.GetTypeEntity(context);
        }
        else
        {
            varType = ExpressionExtensions.TypeOf(varDecl.AssignedValue, context);
        }
        
        if (varType is FuncTypeEntity fe)
        {
            var km = new KnownMethod(
                varDecl.Identifier.SyntaxText,
                fe.ParameterTypeEntities.ToImmutableList(),
                ImmutableList.Create<TypeParam>(), 
                fe.ResultTypeEntity,
                null);
            context.Methods.Add(km);
        }
        
        context.Values.Add(new KnownValue(varDecl.Identifier.SyntaxText, varType, varDecl.Identifier));
    }

    private static void AnalyzeDestructuredDeclStatement(DestructuredVariableDeclarationStatementToken destruct,
        SourceContext context)
    {
        var expType = ExpressionExtensions.TypeOf(destruct.Expression, context);
        if (expType is TupleTypeEntity tup)
        {
            if (tup.ElementTypeEntities.Count != destruct.Identifiers.Count)
            {
                throw new ArgumentException($"Cannot destructure {expType.Name} into {destruct.Identifiers.Count} values.");
            }

            for (int i = 0; i < tup.ElementTypeEntities.Count; i++)
            {
                var id = destruct.Identifiers[i];
                var tupType = tup.ElementTypeEntities[i];
                context.Values.Add(new KnownValue(id.SyntaxText, tupType, id));
            }
        }
        else
        {
            throw new ArgumentException($"Cannot destructure value of type {expType.Name}");
        }
    }
    private static void AnalyzeTypeDeclarationStatement(TypeDeclarationStatementToken typeDecl, SourceContext context)
    {
        var name = ((SimpleIdentifierToken)typeDecl.Identifier).Text;
        var genericContext = context.NewScoped(name);
        foreach (var tp in typeDecl.TypeParameters)
        {
            genericContext.Types.Add(new TypeArgEntity(tp.Identifier, tp));
        }

        var tps = typeDecl.TypeParameters
            .Select(tp => new TypeParam(tp.Identifier))
            .ToImmutableList();
        context.Types.Add(new AliasedTypeEntity(
            name,
            typeDecl.AssignedType.GetTypeEntity(genericContext),
            tps,
            typeDecl.Identifier));
    }

    public static List<StatementToken> ChildStatementsAndSelf(this StatementToken t)
    {
        var ls = new List<StatementToken>();
        ls.Add(t);
        ls.AddRange(ChildStatements(t));
        return ls;
    }
    public static List<StatementToken> ChildStatements(this StatementToken t) => t switch
    {
        BlockStatementToken b => b.Statements.ToList(),
        VariableDeclarationStatementToken => new List<StatementToken>(),
        TypeDeclarationStatementToken => new List<StatementToken>(),
        DestructuredVariableDeclarationStatementToken => new List<StatementToken>(),
        ReturnStatementToken => new List<StatementToken>(),
        _ => throw new ArgumentException($"Unknown statement type: {t}")
    };
}