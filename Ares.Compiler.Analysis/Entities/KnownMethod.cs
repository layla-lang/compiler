using System.Collections.Immutable;
using System.Text;
using Ares.Compiler.Analysis.Entities.Types;
using Ares.Compiler.Analysis.Entities.Types.TypeArgs;
using Ares.Compiler.Tables;
using Ares.Compiler.Tokens;

namespace Ares.Compiler.Analysis.Entities;

public record KnownMethod : ILookupable, ICheckpointable, IScoping
{
    public KnownMethod(
        string methodName,
        IImmutableList<TypeEntity> parameterTypes,
        IImmutableList<TypeParam> typeParameters,
        TypeEntity returnType,
        SourceContext? scopedContext,
        SyntaxToken? token = null)
    {
        this.MethodName = methodName;
        this.TypeArgs = typeParameters.Select(tp => (TypeArg)new OpenTypeArg(tp.Identifier)).ToImmutableList();
        this.ParameterTypes = parameterTypes;
        this.TypeParameters = typeParameters;
        this.TypeArgs = typeParameters.Select(tp => (TypeArg)new OpenTypeArg(tp.Identifier)).ToImmutableList();
        this.ReturnType = returnType;
        this.ScopedContext = scopedContext;
        this.SourceToken = token;
        this.Index = token!.ToCheckpoint();
    }
    
    public string MethodName { get; init; }
    public SyntaxToken? SourceToken { get; init; }
    public CheckpointIndex? Index { get; init; }
    public IImmutableList<TypeEntity> ParameterTypes { get; init; }
    public IImmutableList<TypeParam> TypeParameters { get; init; }
    public TypeEntity ReturnType { get; init; }
    public SourceContext? ScopedContext { get; }
    public string Name => GetKnownMethodName(this);

    public IImmutableList<TypeArg> TypeArgs { get; init; }
    public IEnumerable<OpenTypeArg> OpenTypeArgs => TypeArgs.OfType<OpenTypeArg>();
    public IEnumerable<ClosedTypeArg> ClosedTypeArgs => TypeArgs.OfType<ClosedTypeArg>();
    public int Arity => OpenTypeArgs.Count();
    
    public KnownMethod ProvideTypeArgument(TypeArgEntity tp, TypeEntity v)
    {
        return this with
        {
            ParameterTypes = ParameterTypes.Select(pt =>
                pt % tp
                ? v
                : pt.ProvideTypeArgument(tp, v)).ToImmutableList(),
            TypeArgs = TypeArgs.Select(t => t.Identifier == tp.ParameterName
                ? new ClosedTypeArg(t.Identifier, v)
                : t).ToImmutableList(),
            ReturnType = ReturnType % tp
                ? v
                : ReturnType.ProvideTypeArgument(tp, v),
        };
    }

    public static string GetKnownMethodName(KnownMethod km) =>
        GetKnownMethodName(km.MethodName, km.TypeParameters.Count, km.ParameterTypes);

    public static string GetKnownMethodNameGenericPrefix(string methodName, int typeParamCount)
    {
        var sb = new StringBuilder();
        sb.Append(methodName); // Write method name
        if (typeParamCount > 0)
        {
            sb.Append($"`{typeParamCount}"); // Write arity
        }

        return sb.ToString();
    }
    public static string GetKnownMethodName(string methodName, int typeParamCount, IEnumerable<TypeEntity> parameterTypes)
    {
        var sb = new StringBuilder();
        sb.Append(GetKnownMethodNameGenericPrefix(methodName, typeParamCount));
        
        var pars = parameterTypes.ToList();
        sb.Append("(");
        for (int i = 0; i < pars.Count; i++)
        {
            var p = pars[i];
            sb.Append(p.Name);
            if (i + 1 < pars.Count) sb.AppendLine(",'");
        }
        sb.Append(")");
        return sb.ToString();
    }
}