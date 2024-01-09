using System.Collections.Immutable;
using Ares.Compiler.Analysis.Entities;
using Ares.Compiler.Analysis.Entities.Types;
using Ares.Compiler.Tokens;

namespace Ares.Compiler.Analysis.Helpers;

public static class FuncKnownMethodBuilder
{
    public record PartiallyBuilt(
        Dictionary<string, TypeEntity> Arguments,
        List<TypeParam> TypeParameters,
        SourceContext BodyContext);
    
    public static PartiallyBuilt PartiallyBuild(
        FuncMemberToken token,
        SourceContext startingContext)
    {
        var bodyContext = startingContext.NewScoped(token.Name);
        
        var pars = token.Parameters
            .ToDictionary(
                p => p.Key,
                p => p.Value.GetTypeEntity(startingContext));
        var constraintLookup = token.TypeConstraints;
        var typePars = token.TypeParameters
            .Select(tp =>
                new TypeParam(tp.Key,
                    CollectionExtensions.GetValueOrDefault(constraintLookup, tp.Key, ImmutableList<TypeConstraintToken>.Empty)))
            .ToList();
        
        foreach (var tp in token.TypeParameters)
        {
            var typeArg = new TypeArgEntity(tp.Value.Identifier, tp.Value);
            bodyContext.Types.Add(typeArg);
            if (constraintLookup.ContainsKey(tp.Value.Identifier))
            {
                ApplyConstraintsToContext(bodyContext, typeArg, constraintLookup[tp.Value.Identifier]);
            }
        }
        
        foreach (var tp in pars)
        {
            var kv = new KnownValue(tp.Key, tp.Value, tp.Value.Token);
            bodyContext.Values.Add(kv);

            TypeEntity paramType = kv.Type;
            if (paramType is AliasedTypeEntity a && a.ResolveReference() is FuncTypeEntity f)
            {
                var km = new KnownMethod(tp.Key, f.ParameterTypeEntities,
                    a.TypeParameters, f.ResultTypeEntity, bodyContext);
                bodyContext.Methods.Add(km);
            }
            /*
            else if (paramType is FuncTypeEntity ft)
            {
                var km = new KnownMethod(tp.Key, ft.ParameterTypeEntities,
                    ft.TypeParameters, ft.ResultTypeEntity, bodyContext);
                bodyContext.Methods.Add(km);
            }
            */
        }

        return new PartiallyBuilt(pars, typePars, bodyContext);
    }

    private static void ApplyConstraintsToContext(SourceContext context, TypeArgEntity typeArg,
        IEnumerable<TypeConstraintToken> constraints)
    {
        foreach (var c in constraints) ApplyConstraintToContext(context, typeArg, c);
    }
    
    private static void ApplyConstraintToContext(SourceContext context, TypeArgEntity typeArg,
        TypeConstraintToken constraint)
    {
        switch (constraint.ConstraintType)
        {
            case TypeConstraintType.IsClosedOver:
                var ops = CreateOperators(typeArg, (IsClosedUnderTypeConstraintToken)constraint);
                foreach (var op in ops)
                {
                    context.Operators.Add(op);
                }
                break;
            case TypeConstraintType.Extends:
                var ct = (ExtendsConstraintToken)constraint;
                foreach (var t in ct.Types)
                {
                    var tdEntity = t.GetTypeEntity(context);
                    context.Universe.AddTypeExtendsFact(typeArg, tdEntity);
                }
                break;
        }
    }

    private static List<KnownOperator> CreateOperators(TypeArgEntity te, IsClosedUnderTypeConstraintToken isClosedUnderToken) =>
        isClosedUnderToken.Operators
            .Select(op => CreateOperator(te, op))
            .ToList();

    private static KnownOperator CreateOperator(TypeArgEntity te, string operatorText) => operatorText switch
    {
        "+" => new KnownBinaryOperator(operatorText, te, te, te),
        "-" => new KnownBinaryOperator(operatorText, te, te, te),
        "*" => new KnownBinaryOperator(operatorText, te, te, te),
        "/" => new KnownBinaryOperator(operatorText, te, te, te),
        "^" => new KnownBinaryOperator(operatorText, te, te, te),
        "%" => new KnownBinaryOperator(operatorText, te, te, te),
        ">" => new KnownBinaryOperator(operatorText, te, te, TypeEntity.Bool),
        ">=" => new KnownBinaryOperator(operatorText, te, te, TypeEntity.Bool),
        "<" => new KnownBinaryOperator(operatorText, te, te, TypeEntity.Bool),
        "<=" => new KnownBinaryOperator(operatorText, te, te, TypeEntity.Bool),
        "==" => new KnownBinaryOperator(operatorText, te, te, TypeEntity.Bool),
        "!=" => new KnownBinaryOperator(operatorText, te, te, TypeEntity.Bool),
        _ => throw new ArgumentException($"Unknown operator '{operatorText}'.")
    };
}