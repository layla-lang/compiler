using System.Collections.Immutable;
using Ares.Compiler.Parser.Syntax;
using Ares.Compiler.Tokens;

namespace Ares.Compiler.Transformer;

public class ContextTransformer
{
    public static ContextToken TransformContext(Context.ContextSyntaxElement ele)
    {
        var originalMembers = ele.Body.ToList();
        var transformedMembers = ele.Body
            .Select(MemberTransformer.TransformMember)
            .ToImmutableList();
        var token = new ContextToken(ele.Name, transformedMembers);

        for (int i = 0; i < originalMembers.Count; i++)
        {
            var orig = originalMembers[i];
            var transformed = transformedMembers[i];
            transformed.Parent = token;
            transformed.Slice = SourceSlice.FromSyntaxElementAndCode(orig, () => token.Code);
        }
        
        return token;
    }
}