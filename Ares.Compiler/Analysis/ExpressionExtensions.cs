using System.Collections.Immutable;
using Ares.Compiler.Analysis.Entities;
using Ares.Compiler.Analysis.Entities.Types;
using Ares.Compiler.Analysis.Helpers;
using Ares.Compiler.Analysis.Tables;
using Ares.Compiler.Tokens;

namespace Ares.Compiler.Analysis;

public static class ExpressionExtensions
{
    public static TypeEntity TypeOf(ExpressionToken e, SourceContext context) => e switch
    {
        ConstantExpressionToken c => c.Value.GetGeneralTypeEntityOfLiteral(),
        VariableExpressionToken v => TypeFromVar(v, context),
        CastExpressionToken c => c.TypeDescriptor.GetTypeEntity(context),
        TernaryExpressionToken t => TypeFromTernary(t, context),
        OperationExpressionToken o => TypeFromOperator(o, context),
        LambdaExpressionToken l => TypeFromLambda(l, context),
        InvocationExpressionToken i => TypeFromInvocation(i, context),
        ObjectExpressionToken i => TypeFromObject(i, context),
        TupleExpressionToken t => TypeFromTuple(t, context),
        ArrayExpressionToken t => TypeFromArray(t, context),
        IsTypeExpressionToken t => TypeEntity.Bool,
        _ => throw new ArgumentException($"Unknown expression type.")
    };

    private static TypeEntity TypeFromVar(VariableExpressionToken v, SourceContext context)
    {
        if (v.Identifier is MemberAccessIdentifierToken m)
        {
            var id = string.Join(".", m.PathIdentifiers
                .Select(id => id.SyntaxText));
            var mkt = context.Values[id];
            if (!mkt.HasValue)
            {
                throw new ArgumentException($"Unknown constant expression token: {v}");
            }
            return TypeFromMember(mkt.Value.Result.Type!, m.MemberIdentifier.SyntaxText, context);
        }
        else if (v.Identifier is IndexedAccessIdentifierToken i)
        {
            var arrName = i.Identifier;
            var arrVal = context.Values[arrName.Text]!.Value.Result;
            var expType = TypeOf(i.AccessExpression, context);
            return TypeDescriptorExtensions.TypeOfIndexedType(arrVal.Type, expType, context);
        }
        var kt = context.Values[v.Identifier];
        if (!kt.HasValue)
        {
            throw new ArgumentException($"Unknown constant expression token: {v}");
        }

        return kt.Value.Result.Type;
    }

    private static TypeEntity TypeFromTernary(TernaryExpressionToken t, SourceContext context)
    {
        var leftType = TypeOf(t.IfTrueExpression, context);
        var rightType = TypeOf(t.IfFalseExpression, context);
        return UnionTypeEntity.CreateTypeUnion(new List<TypeEntity>(new[]
        {
            leftType,
            rightType
        }), t);
    }
    
    private static TypeEntity TypeFromOperator(OperationExpressionToken token, SourceContext context)
    {
        var leftType = TypeOf(token.Left, context);
        var rightType = TypeOf(token.Right, context);
        var operatorName = KnownBinaryOperator.GetName(
            token.Operator.GetSymbol(),
            leftType,
            rightType);
        
        var op = context.Operators[operatorName];
        if (!op.HasValue)
        {
            throw new ArgumentException($"Unknown operator {token.Operator} for types {leftType} and {rightType}.");
        }

        return op.Value.Result.ResultType;
    }
    
    private static TypeEntity TypeFromLambda(LambdaExpressionToken token, SourceContext context)
    {
        var parTypes = new List<TypeEntity>();
        var childContext = context.NewScoped("_lambda");
        foreach (var p in token.Parameters)
        {
            if (p.Type is SpecifiedLambdaParameterTypeToken s)
            {
                var parType = s.TypeDescriptor.GetTypeEntity(context);
                parTypes.Add(parType);
                childContext.Values.Add(new KnownValue(p.Identifier.SyntaxText, parType, token));
            }
            else
            {
                
            }
        }
        var bodyType = TypeOf(token.Expression, childContext);
        return new FuncTypeEntity(parTypes.ToImmutableList(), bodyType, token);
    }
    private static TypeEntity TypeFromInvocation(InvocationExpressionToken token, SourceContext context)
    {
        var methodResponse = InvocationHelper.FindInvokedMethod(
            context,
            token.ToResolveMethodRequest(context));
        if (methodResponse == null)
        {
            throw new ArgumentException($"Unknown method {token.Identifier.SyntaxText}.");
        }

        var method = methodResponse.BoundMethod;
        return method.ReturnType;
    }
    
    private static TypeEntity TypeFromObject(ObjectExpressionToken token, SourceContext context)
    {
        var members = new Dictionary<string, TypeEntity>();
        foreach (var m in token.Members)
        {
            members.Add(m.Identifier.SyntaxText, TypeOf(m.Value, context));
        }

        return new RecordTypeEntity(members, token);
    }
    
    private static TypeEntity TypeFromTuple(TupleExpressionToken token, SourceContext context)
    {
        var elements = new List<TypeEntity>();
        foreach (var m in token.Elements)
        {
            elements.Add(TypeOf(m, context));
        }

        return new TupleTypeEntity(elements.ToImmutableList(), token);
    }
    private static TypeEntity TypeFromArray(ArrayExpressionToken token, SourceContext context)
    {
        var elements = token.Items
            .Select(i => TypeOf(i, context))
            .DistinctBy(i => i.Name)
            .ToList();

        if (elements.Count == 0)
        {
            throw new ArgumentException($"Unable to infer array type since array is empty.");
        }
        if (elements.Count > 1)
        {
            throw new ArgumentException($"Unable to infer array type since elements are of different types.");
        }
        return new ArrayTypeEntity(elements.First(), token);
    }
    
    public static string GetSymbol(this Operator op) => op switch
    {
        Operator.Multiplication => "*",
        Operator.Addition => "+",
        Operator.Subtraction => "-",
        Operator.Power => "^",
        Operator.Modulus => "%",
        Operator.Gt => ">",
        Operator.Gte => ">=",
        Operator.Lt => "<",
        Operator.Lte => "<=",
        Operator.Eq => "==",
        Operator.NotEq => "!=",
        _ => throw new ArgumentException($"Unknown operator: {op}")
    };
    
    private static TypeEntity TypeFromMember(TypeEntity t, string member, SourceContext context)
    {
        if (t is RecordTypeEntity r)
        {
            var m = r.Members[member];
            return m;
        }
        else
        {
            throw new ArgumentException($"Cannot get type of member '{member}' on type {t}.");
        }
    }
}