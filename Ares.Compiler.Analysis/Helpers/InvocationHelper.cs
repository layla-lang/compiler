using Ares.Compiler.Analysis.Entities;
using Ares.Compiler.Analysis.Entities.Types;
using Ares.Compiler.Analysis.Entities.Types.TypeArgs;

namespace Ares.Compiler.Analysis.Helpers;

public static class InvocationHelper
{
    public record ResolveMethodRequest(
        string MethodName,
        List<TypeEntity> TypeArguments,
        List<TypeEntity> Arguments);

    public record ResolveMethodResponse(
        KnownMethod UnboundMethod,
        KnownMethod BoundMethod);
    public static ResolveMethodResponse FindInvokedMethod(
        SourceContext context,
        ResolveMethodRequest request)
    {
        var methodName = request.MethodName;
        var arguments = request.Arguments;
        var typeArguments = request.TypeArguments;

        bool TryBindExactly(KnownMethod unboundMethod, out KnownMethod? boundMethod)
        {
            KnownMethod specificMethod = unboundMethod;
            boundMethod = null;
            
            for (int i = 0; i < specificMethod.TypeParameters.Count; i++)
            {
                var p = specificMethod.TypeParameters[i];
                var a = typeArguments[i];
                var tpe = new TypeArgEntity(p.Identifier, null);
                specificMethod = specificMethod.ProvideTypeArgument(tpe, a);
            }

            for (int i = 0; i < specificMethod.ParameterTypes.Count; i++)
            {
                var smPt = specificMethod.ParameterTypes[i];
                var arg = arguments[i];
                if (smPt != arg) return false;
            }

            boundMethod = specificMethod;
            return true;
        }

        bool TryBindInferred(KnownMethod unboundMethod, out KnownMethod? boundMethod)
        {
            var inferRequest = new TypeArgInferenceRequest(unboundMethod, arguments);
            var inferResponse = InferTypeArguments(inferRequest);
            if (inferResponse.IsClosed)
            {
                boundMethod = inferResponse.BindInferredTypes();
                return true;
            }

            boundMethod = null;
            return false;
        }

        int arity = typeArguments.Count;
        var genericPrefix = KnownMethod.GetKnownMethodNameGenericPrefix(methodName, arity);
        var genericPrefixMatches = context.Methods.ScanByPrefix(genericPrefix + "(")
            .Where(m => m.Result.ParameterTypes.Count == arguments.Count)
            .ToList();
        foreach (var m in genericPrefixMatches)
        {
            var method = m.Result;
            if (TryBindExactly(method, out var boundMethod))
            {
                return new ResolveMethodResponse(method, boundMethod!);
            }
        }

        var namePrefixMatches = context.Methods.ScanByPrefix(methodName)
            .Where(m => m.Result.MethodName == methodName)
            .Where(m => m.Result.ParameterTypes.Count == arguments.Count)
            .ToList();
        foreach (var m in namePrefixMatches)
        {
            var method = m.Result;
            if (TryBindInferred(method, out var boundMethod))
            {
                return new ResolveMethodResponse(method, boundMethod!);
            }
        }

        return null;
    }

    public record TypeArgInferenceRequest(
        KnownMethod Method,
        List<TypeEntity> Arguments);
    public record TypeArgInferenceResult(
        KnownMethod OriginalMethod,
        List<ClosedTypeArg> NewlyInferredTypes)
    {
        public bool IsClosed => NewlyInferredTypes.Count == OriginalMethod.Arity;
        public KnownMethod BindInferredTypes()
        {
            KnownMethod km = OriginalMethod;
            foreach (var i in NewlyInferredTypes)
            {
                km = km.ProvideTypeArgument(
                    new TypeArgEntity(i.Identifier, null),
                    i.ClosingType);
            }

            return km;
        }
    }
    
    public static TypeArgInferenceResult InferTypeArguments(
        TypeArgInferenceRequest request)
    {
        var method = request.Method;
        var arguments = request.Arguments;
        var openArgs = method.OpenTypeArgs.ToDictionary(a => a.Identifier, a => a);
        var closedArgs = method.ClosedTypeArgs.ToDictionary(a => a.Identifier, a => a);
        var newlyInferred = new List<ClosedTypeArg>();
        for (int i = 0; i < method.ParameterTypes.Count; i++)
        {
            var mArt = method.ParameterTypes[i];
            var arg = arguments[i];
            
            var inferResult = InferTypeArgsFromArg(mArt, arg);
            foreach (var r in inferResult)
            {
                if (closedArgs.ContainsKey(r.Key))
                {
                    if (closedArgs[r.Key].ClosingType != inferResult[r.Key])
                    {
                        throw new ArgumentException($"Same parameter {r.Key} inferred to two different values!");
                    }

                    continue;
                }
                var nit = new ClosedTypeArg(r.Key, r.Value!);
                newlyInferred.Add(nit);
                openArgs.Remove(r.Key);
                closedArgs.Add(r.Key, nit);
            }
        }

        return new TypeArgInferenceResult(method, newlyInferred);
    }

    internal static Dictionary<string, TypeEntity> InferTypeArgsFromArg(TypeEntity declaredArg, TypeEntity providedArg)
    {
        var result = new Dictionary<string, TypeEntity>();
        if (declaredArg is TypeArgEntity te)
        {
            result.Add(te.ParameterName, providedArg);
            return result;
        }
        else if (declaredArg is ArrayTypeEntity declaredArr && providedArg is ArrayTypeEntity providedArr)
        {
            return InferTypeArgsFromArg(declaredArr.ElementType, providedArr.ElementType);
        }
        else if (declaredArg is TupleTypeEntity declaredTup && providedArg is TupleTypeEntity providedTup)
        {
            if (declaredTup.ElementTypeEntities.Count == providedTup.ElementTypeEntities.Count)
            {
                for (int i = 0; i < providedTup.ElementTypeEntities.Count; i++)
                {
                    var declaredTupEle = declaredTup.ElementTypeEntities[i];
                    var providedTupEle = providedTup.ElementTypeEntities[i];
                    var eleResult = InferTypeArgsFromArg(declaredTupEle, providedTupEle);
                    result.MergeIfSame(eleResult);
                }
            }
        }
        else if (declaredArg is FuncTypeEntity declaredFunc && providedArg is FuncTypeEntity providedFunc)
        {
            if (declaredFunc.ParameterTypeEntities.Count == providedFunc.ParameterTypeEntities.Count)
            {
                for (int i = 0; i < declaredFunc.ParameterTypeEntities.Count; i++)
                {
                    var declaredParamEle = declaredFunc.ParameterTypeEntities[i];
                    var providedParamEle = providedFunc.ParameterTypeEntities[i];
                    var eleResult = InferTypeArgsFromArg(declaredParamEle, providedParamEle);
                    result.MergeIfSame(eleResult);
                }

                var returnTypeResult =
                    InferTypeArgsFromArg(declaredFunc.ResultTypeEntity, providedFunc.ResultTypeEntity);
                result.MergeIfSame(returnTypeResult);
            }
        }
        else if (declaredArg is AliasedTypeEntity alias)
        {
            return InferTypeArgsFromArg(alias.ResolveReference(), providedArg);
        }
        return result;
    }

    private static void MergeIfSame(this Dictionary<string, TypeEntity> typeDict, Dictionary<string, TypeEntity> other)
    {
        foreach (var kvp in other)
        {
            if (typeDict.ContainsKey(kvp.Key))
            {
                if (typeDict[kvp.Key] == other[kvp.Key]) continue;
                throw new ArgumentException($"Duplicate key {kvp.Key} with different value!");
            }
            typeDict.Add(kvp.Key, other[kvp.Key]);
        }
    }
}