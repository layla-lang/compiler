using Ares.Compiler.Analysis.Entities;
using Ares.Compiler.Analysis.Entities.Types;

namespace Ares.Compiler.Analysis.Helpers;

public class GeneralizationHelper
{
    public static TypeEntity Generalize(List<TypeEntity> types, SourceContext context)
    {
        if (types.Count == 0) return TypeEntity.Never;
        var deduped = types.Distinct().ToList();
        if (deduped.OfType<AnyTypeEntity>().Any()) return TypeEntity.Any;
        if (deduped.Count == 1)
        {
            return deduped[0];
        }

        var queue = new Queue<TypeEntity>(deduped);
        TypeEntity result = queue.Dequeue();
        TypeEntity right;
        while (queue.Count > 0)
        {
            right = queue.Dequeue();
            result = context.Universe.LowestCommonAncestor(result, right);
        }

        return result;
        
        //return UnionTypeEntity.CreateTypeUnion(deduped, null);
    }

    public static TypeEntity Generalize(SourceContext context, params TypeEntity[] types) =>
        Generalize(types.ToList(), context);
}