using System.Collections.Immutable;
using Ares.Compiler.Parser.Syntax;
using Ares.Compiler.Tokens;

namespace Ares.Compiler.Transformer;

public class TypeConstraintTransformer
{
    public static TypeConstraintToken TransformTypeConstraint(TypeConstraint.TypeConstraintSyntaxElement ele) =>
        ele.TypeConstraint.Tag switch
        {
            TypeConstraint.TypeConstraint.Tags.IsClosedUnder => TransformHasConstraint((TypeConstraint.TypeConstraint.IsClosedUnder)ele.TypeConstraint),
            TypeConstraint.TypeConstraint.Tags.Extends => TransformExtendsConstraint((TypeConstraint.TypeConstraint.Extends)ele.TypeConstraint),
            _ => throw new ArgumentException($"Unknown type constraint type: {ele}")
        };

    private static TypeConstraintToken TransformHasConstraint(TypeConstraint.TypeConstraint.IsClosedUnder ele)
    {
        var operators = ele.Operators.ToImmutableList();
        return new IsClosedUnderTypeConstraintToken(operators);
    }
    private static ExtendsConstraintToken TransformExtendsConstraint(TypeConstraint.TypeConstraint.Extends ele)
    {
        var origTypes = ele.Item.ToList();
        var transformedTypes = origTypes
            .Select(TypeDescriptorTransformer.TransformTypeDescriptor)
            .ToImmutableList();
        var token = new ExtendsConstraintToken(transformedTypes);
        for (int i = 0; i < origTypes.Count; i++)
        {
            var orig = origTypes[i];
            var transformed = transformedTypes[i];
            transformed.Parent = token;
            transformed.Slice = SourceSlice.FromSyntaxElementAndCode(orig, () => token.Code);
        }

        return token;
    }
}