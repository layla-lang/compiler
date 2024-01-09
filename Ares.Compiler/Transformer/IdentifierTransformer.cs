using System.Collections.Immutable;
using Ares.Compiler.Parser.Syntax;
using Ares.Compiler.Tokens;

namespace Ares.Compiler.Transformer;

public static class IdentifierTransformer
{
    public static IdentifierToken TransformIdentifier(Expression.IdentifierSyntaxElement id)
    {
        IdentifierToken idToken = id.Identifier.Tag switch
        {
            Expression.Identifier.Tags.Simple => TransformSimpleIdentifierToken((Expression.Identifier.Simple)id.Identifier),
            Expression.Identifier.Tags.IndexAccess => TransformIndexedAccessIdentifierToken(
                (Expression.Identifier.IndexAccess)id.Identifier),
            Expression.Identifier.Tags.MemberAccess => TransformMemberAccessIdentifierToken(
                (Expression.Identifier.MemberAccess)id.Identifier),
            _ => throw new ArgumentException($"Unknown identifier type: {id}")
        };
        return idToken;
    }

    public static SimpleIdentifierToken TransformSimpleIdentifierToken(Expression.Identifier.Simple id) =>
        new SimpleIdentifierToken(id.Item);

    public static IndexedAccessIdentifierToken TransformIndexedAccessIdentifierToken(
        Expression.Identifier.IndexAccess ia)
    {
        var id = (SimpleIdentifierToken)TransformIdentifier(ia.Item1);
        var expr = ExpressionTransformer.TransformExpression(ia.Item2);
        var token = new IndexedAccessIdentifierToken(id, expr);
        id.Parent = token;
        id.Slice = SourceSlice.FromSyntaxElementAndCode(ia.Item1, () => token.Code);
        expr.Parent = token;
        expr.Slice = SourceSlice.FromSyntaxElementAndCode(ia.Item2, () => token.Code);
        return token;
    }
    
    public static MemberAccessIdentifierToken TransformMemberAccessIdentifierToken(
        Expression.Identifier.MemberAccess ma)
    {
        var pathIds = ImmutableList.Create<IdentifierToken>()
            .AddRange(ma.Item1.Select(TransformIdentifier).ToList());
        var accessId = TransformIdentifier(ma.Item2);
        var maid = new MemberAccessIdentifierToken(pathIds, accessId);
        for (var i = 0; i < pathIds.Count; i++)
        {
            var oid = ma.Item1[i];
            var nid = pathIds[i];
            nid.Parent = maid;
            nid.Slice = SourceSlice.FromSyntaxElementAndCode(oid, () => maid.Code);
        }

        accessId.Parent = maid;
        accessId.Slice = SourceSlice.FromSyntaxElementAndCode(ma.Item2, () => maid.Code);
        return maid;
    }
}