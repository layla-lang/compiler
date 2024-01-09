using System.Collections.Immutable;
using Ares.Compiler.Parser.Syntax;
using Ares.Compiler.Tokens;

namespace Ares.Compiler.Transformer;

public class StatementTransformer
{
    public static StatementToken TransformStatement(Statement.StatementSyntaxElement stmtStx) => stmtStx.Statement.Tag switch
    {
        Statement.StmtStx.Tags.VariableDeclaration => TransformVariableDeclarationSyntax(
            (Statement.StmtStx.VariableDeclaration)stmtStx.Statement),
        Statement.StmtStx.Tags.DestructuringVariableDeclaration => TransformDestructuredSyntax(
            (Statement.StmtStx.DestructuringVariableDeclaration)stmtStx.Statement),
        Statement.StmtStx.Tags.TypeDeclaration => TransformTypeDeclarationSyntax(
            (Statement.StmtStx.TypeDeclaration)stmtStx.Statement),
        Statement.StmtStx.Tags.Block => TransformBlockStatementSyntax(
            (Statement.StmtStx.Block)stmtStx.Statement),
        Statement.StmtStx.Tags.Return => TransformReturnStatementSyntax(
            (Statement.StmtStx.Return)stmtStx.Statement),
        _ => throw new ArgumentException($"Unknown statement type: {stmtStx}")
    };

    private static VariableDeclarationStatementToken TransformVariableDeclarationSyntax(Statement.StmtStx.VariableDeclaration decl)
    {
        var varIdentifier = IdentifierTransformer.TransformSimpleIdentifierToken((Expression.Identifier.Simple)decl.Identifier.Identifier);
        DeclarationTypeToken declaredType = decl.DeclaredType.IsInferred
            ? new InferredDeclaredTypeToken()
            : new TypeDescriptorTypeToken(TypeDescriptorTransformer.TransformTypeDescriptor(((Statement.DeclaredType.TypeDescriptor)decl.DeclaredType).Item));
        var assignedValueExpr = ExpressionTransformer.TransformExpression(decl.AssignedValue);

        var token = new VariableDeclarationStatementToken(varIdentifier, declaredType, assignedValueExpr);
        varIdentifier.Parent = token;
        varIdentifier.Slice = SourceSlice.FromSyntaxElementAndCode(decl.Identifier, () => token.Code);
        if (declaredType is TypeDescriptorTypeToken tdTt)
        {
            tdTt.TypeDescriptor.Parent = token;
            tdTt.TypeDescriptor.Slice = SourceSlice.FromSyntaxElementAndCode(((Statement.DeclaredType.TypeDescriptor)decl.DeclaredType).Item, () => token.Code);
        }

        assignedValueExpr.Parent = token;
        assignedValueExpr.Slice = SourceSlice.FromSyntaxElementAndCode(decl.AssignedValue, () => token.Code);
        return token;
    }

    private static DestructuredVariableDeclarationStatementToken TransformDestructuredSyntax(
        Statement.StmtStx.DestructuringVariableDeclaration destruct)
    {
        var originalIds = destruct.Identifiers.ToList();
        var transformedIds = originalIds
            .Select(i => IdentifierTransformer.TransformIdentifier(i))
            .ToImmutableList();
        var exp = ExpressionTransformer.TransformExpression(destruct.AssignedValue);
        var token = new DestructuredVariableDeclarationStatementToken(transformedIds, exp);
        exp.Parent = token;
        exp.Slice = SourceSlice.FromSyntaxElementAndCode(destruct.AssignedValue, () => token.Code);
        for (int i = 0; i < originalIds.Count; i++)
        {
            var original = originalIds[i];
            var transformed = transformedIds[i];
            transformed.Parent = token;
            transformed.Slice = SourceSlice.FromSyntaxElementAndCode(original, () => token.Code);
        }

        return token;
    }
    
    private static TypeDeclarationStatementToken TransformTypeDeclarationSyntax(Statement.StmtStx.TypeDeclaration decl)
    {
        var typeIdentifier = IdentifierTransformer.TransformSimpleIdentifierToken((Expression.Identifier.Simple)decl.Item1.Identifier);
        var typeParams = ImmutableList.Create<TypeParameterToken>()
            .AddRange(decl.Item2.Select(TypeParameterTransformer.TransformTypeParameter).ToList());
        var assignedTypeExpr = TypeDescriptorTransformer.TransformTypeDescriptor(decl.Item3);

        var token = new TypeDeclarationStatementToken(typeIdentifier, typeParams, assignedTypeExpr);
        typeIdentifier.Parent = token;
        typeIdentifier.Slice = SourceSlice.FromSyntaxElementAndCode(decl.Item1, () => token.Code);

        for (int i = 0; i < typeParams.Count; i++)
        {
            var ip = decl.Item2[i];
            var tp = typeParams[i];
            tp.Parent = token;
            tp.Slice = SourceSlice.FromSyntaxElementAndCode(ip, () => token.Code);
        }

        assignedTypeExpr.Parent = token;
        assignedTypeExpr.Slice = SourceSlice.FromSyntaxElementAndCode(decl.Item3, () => token.Code);
        
        return token;
    }
    
    private static BlockStatementToken TransformBlockStatementSyntax(Statement.StmtStx.Block b)
    {
        var blockStatements = b.Item.Select(TransformStatement).ToList();
        var token = new BlockStatementToken(ImmutableList
            .Create<StatementToken>()
            .AddRange(blockStatements));
        for (int i = 0; i < blockStatements.Count; i++)
        {
            var stmt = blockStatements[i];
            var o = b.Item[i];
            stmt.Parent = token;
            stmt.Slice = SourceSlice.FromSyntaxElementAndCode(o, () => token.Code);
        }

        return token;
    }
    
    private static ReturnStatementToken TransformReturnStatementSyntax(Statement.StmtStx.Return b)
    {
        var transformedExp = ExpressionTransformer.TransformExpression(b.Item);
        var token = new ReturnStatementToken(transformedExp);
        transformedExp.Parent = token;
        transformedExp.Slice = SourceSlice.FromSyntaxElementAndCode(b.Item, () => token.Code);
        return token;
    }
}