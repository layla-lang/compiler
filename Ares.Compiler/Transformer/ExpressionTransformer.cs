using System.Collections.Immutable;
using Ares.Compiler.Tokens;
using Expression = Ares.Compiler.Parser.Syntax.Expression;

namespace Ares.Compiler.Transformer;

public class ExpressionTransformer
{
    public static ExpressionToken TransformExpression(Expression.ExpressionSyntaxElement exp) => exp.Expression.Tag switch
        {
            Expression.Expression.Tags.Operation => TransformBinaryOperation(
                (Expression.Expression.Operation)exp.Expression),
            Expression.Expression.Tags.Constant => TransformConstExpression(
                (Expression.Expression.Constant)exp.Expression),
            Expression.Expression.Tags.Variable => TransformVariableExpression(
                (Expression.Expression.Variable)exp.Expression),
            Expression.Expression.Tags.Lambda => TransformLambdaExpression(
                (Expression.Expression.Lambda)exp.Expression),
            Expression.Expression.Tags.Invocation => TransformInvocationExpression(
                (Expression.Expression.Invocation)exp.Expression),
            Expression.Expression.Tags.Ternary => TransformTernaryExpression(
                (Expression.Expression.Ternary)exp.Expression),
            Expression.Expression.Tags.Cast => TransformCastExpression(
                (Expression.Expression.Cast)exp.Expression),
            Expression.Expression.Tags.IsType => TransformIsTypeExpression(
                (Expression.Expression.IsType)exp.Expression),
            Expression.Expression.Tags.Object => TransformObjectExprParameter(
                (Expression.Expression.Object)exp.Expression),
            Expression.Expression.Tags.Tuple => TransformTupleExpression(
                (Expression.Expression.Tuple)exp.Expression),
            Expression.Expression.Tags.Array => TransformArrayExpression(
                (Expression.Expression.Array)exp.Expression),
            _ => throw new ArgumentException($"Unknown expression type: {exp}"),
        };

    internal static OperationExpressionToken TransformBinaryOperation(Expression.Expression.Operation bin)
    {
        var (left, op, right) = bin.Item;

        var leftExp = TransformExpression(left);
        var rightExp = TransformExpression(right);
        
        var operationExp = new OperationExpressionToken(
            leftExp,
            ToLocalOperator(op),
            rightExp);
        leftExp.Parent = operationExp;
        leftExp.Slice = SourceSlice.FromSyntaxElementAndCode(left, () => operationExp.Code);
        rightExp.Parent = operationExp;
        rightExp.Slice = SourceSlice.FromSyntaxElementAndCode(right, () => operationExp.Code);
        return operationExp;
    }

    internal static Operator ToLocalOperator(Expression.Operator op) => op.Tag switch
    {
        Expression.Operator.Tags.Addition => Operator.Addition,
        Expression.Operator.Tags.Subtraction => Operator.Subtraction,
        Expression.Operator.Tags.Multiplication => Operator.Multiplication,
        Expression.Operator.Tags.Division => Operator.Division,
        Expression.Operator.Tags.Modulus => Operator.Modulus,
        Expression.Operator.Tags.Power => Operator.Power,
        Expression.Operator.Tags.Equals => Operator.Eq,
        Expression.Operator.Tags.NotEquals => Operator.NotEq,
        Expression.Operator.Tags.Gt => Operator.Gt,
        Expression.Operator.Tags.Gte => Operator.Gte,
        Expression.Operator.Tags.Lt => Operator.Lt,
        Expression.Operator.Tags.Lte => Operator.Lte,
        _ => throw new ArgumentException($"Unknown operator kind: {op}")
    };

    internal static ConstantExpressionToken TransformConstExpression(Expression.Expression.Constant cExpr)
    {
        var vt = ValueTransformer.TransformValue(cExpr.Item);
        var constExp = new ConstantExpressionToken(vt);
        vt.Parent = constExp;
        vt.Slice = SourceSlice.FromSyntaxElementAndCode(cExpr.Item, () => constExp.Code);
        return constExp;
    }

    internal static VariableExpressionToken TransformVariableExpression(Expression.Expression.Variable v)
    {
        var identifier = IdentifierTransformer.TransformIdentifier(v.Item);
        var varExp = new VariableExpressionToken(identifier);
        identifier.Parent = varExp;
        identifier.Slice = SourceSlice.FromSyntaxElementAndCode(v.Item, () => varExp.Code);
        return varExp;
    }

    internal static LambdaExpressionToken TransformLambdaExpression(Expression.Expression.Lambda lambdaExp)
    {
        var parameterTokens = ImmutableList.Create<LambdaParameterToken>()
            .AddRange(lambdaExp.Parameters.Select(TransformLambdaExprParameter).ToList());
        var expr = TransformExpression(lambdaExp.Expression);
        var lambdaExpr = new LambdaExpressionToken(parameterTokens, expr);

        var llIds = lambdaExp.Parameters.ToList();
        for (int i = 0; i < llIds.Count; i++)
        {
            var lp = llIds[i];
            var transformed = parameterTokens[i];
            transformed.Parent = lambdaExpr;
            SourceSlice slice;
            if (transformed.Type is SpecifiedLambdaParameterTypeToken td)
            {
                var cs = lp.Identifier.CodeSpan.Start;
                slice = SourceSlice.FromSourcePosition(
                    new SourcePosition((int)cs.Line, (int)cs.Column, (int)cs.Index),
                    td.TypeDescriptor.Slice!.End,
                    () => lambdaExpr.Code);
            }
            else
            {
                slice = SourceSlice.FromSyntaxElementAndCode(lp.Identifier, () => lambdaExpr.Code);
            }

            transformed.Slice = slice;
        }

        expr.Parent = lambdaExpr;
        expr.Slice = SourceSlice.FromSyntaxElementAndCode(lambdaExp.Expression, () => lambdaExpr.Code);
        return lambdaExpr;
    }
    
    internal static InvocationExpressionToken TransformInvocationExpression(Expression.Expression.Invocation invocationExp)
    {
        var idToken = IdentifierTransformer.TransformIdentifier(invocationExp.Identifier);
        var typeArgsTokens = ImmutableList.Create<TypeDescriptorToken>()
            .AddRange(invocationExp.TypeArguments
                .Select(TypeDescriptorTransformer.TransformTypeDescriptor).ToList());
        var parameterExpressionTokens = ImmutableList.Create<ExpressionToken>()
            .AddRange(invocationExp.Parameters.Select(TransformExpression));
        var token = new InvocationExpressionToken(idToken, typeArgsTokens, parameterExpressionTokens);

        idToken.Parent = token;
        idToken.Slice = SourceSlice.FromSyntaxElementAndCode(invocationExp.Identifier, () => token.Code);
        
        var typeArgExps = invocationExp.TypeArguments.ToList();
        for (int i = 0; i < typeArgExps.Count; i++)
        {
            var id = typeArgExps[i];
            var transformed = typeArgsTokens[i];
            transformed.Parent = token;
            transformed.Slice = SourceSlice.FromSyntaxElementAndCode(id, () => token.Code);
        }
        
        var parExps = invocationExp.Parameters.ToList();
        for (int i = 0; i < parExps.Count; i++)
        {
            var id = parExps[i];
            var transformed = parameterExpressionTokens[i];
            transformed.Parent = token;
            transformed.Slice = SourceSlice.FromSyntaxElementAndCode(id, () => token.Code);
        }

        return token;
    }
    
    internal static TernaryExpressionToken TransformTernaryExpression(Expression.Expression.Ternary v)
    {
        var predicate = TransformExpression(v.Predicate);
        var ifTrue = TransformExpression(v.IfTrue);
        var ifFalse = TransformExpression(v.IfFalse);
        var token = new TernaryExpressionToken(predicate, ifTrue, ifFalse);
        predicate.Parent = token;
        predicate.Slice = SourceSlice.FromSyntaxElementAndCode(v.Predicate, () => token.Code);
        ifTrue.Parent = token;
        ifTrue.Slice = SourceSlice.FromSyntaxElementAndCode(v.IfTrue, () => token.Code);
        ifFalse.Parent = token;
        ifFalse.Slice = SourceSlice.FromSyntaxElementAndCode(v.IfFalse, () => token.Code);
        return token;
    }
    
    internal static CastExpressionToken TransformCastExpression(Expression.Expression.Cast v)
    {
        var newType = TypeDescriptorTransformer.TransformTypeDescriptor(v.NewType);
        var exp = TransformExpression(v.Expression);
        var token = new CastExpressionToken(newType, exp);
        newType.Parent = token;
        newType.Slice = SourceSlice.FromSyntaxElementAndCode(v.NewType, () => token.Code);
        exp.Parent = token;
        exp.Slice = SourceSlice.FromSyntaxElementAndCode(v.Expression, () => token.Code);
        return token;
    }

    internal static IsTypeExpressionToken TransformIsTypeExpression(Expression.Expression.IsType v)
    {
        var exp = TransformExpression(v.Expression);
        var checkType = TypeDescriptorTransformer.TransformTypeDescriptor(v.TypeDescriptor);
        var token = new IsTypeExpressionToken(exp, checkType);
        exp.Parent = token;
        exp.Slice = SourceSlice.FromSyntaxElementAndCode(v.Expression, () => token.Code);
        checkType.Parent = token;
        checkType.Slice = SourceSlice.FromSyntaxElementAndCode(v.TypeDescriptor, () => token.Code);
        return token;
    }

    private static LambdaParameterToken TransformLambdaExprParameter(Expression.LambdaParameter lp)
    {
        var id = IdentifierTransformer.TransformIdentifier(lp.Identifier);
        LambdaParameterTypeToken lt;
        if (lp.Type.Tag == Expression.LambdaParameterType.Tags.Inferred)
        {
            lt = new LambdaInferredParameterTypeToken();
        }
        else
        {
            Expression.LambdaParameterType.TypeDescriptor td = (Expression.LambdaParameterType.TypeDescriptor)lp.Type;
            TypeDescriptorToken tdToken = TypeDescriptorTransformer.TransformTypeDescriptor(td.Item);
            lt = new SpecifiedLambdaParameterTypeToken(tdToken);
        }
        var token = new LambdaParameterToken(id, lt);
        
        if (lp.Type.Tag == Expression.LambdaParameterType.Tags.TypeDescriptor)
        {
            ((SpecifiedLambdaParameterTypeToken)token.Type).TypeDescriptor.Parent = token;
            ((SpecifiedLambdaParameterTypeToken)token.Type).TypeDescriptor.Slice = SourceSlice.FromSyntaxElementAndCode(
                ((Expression.LambdaParameterType.TypeDescriptor)lp.Type).Item,
                () => token.Code);
        }

        token.Identifier.Parent = token;
        token.Identifier.Slice = SourceSlice.FromSyntaxElementAndCode(lp.Identifier, () => token.Code);
        return token;
    }
    
    private static ObjectExpressionToken TransformObjectExprParameter(Expression.Expression.Object obj)
    {
        var members = new List<ObjectMemberToken>();
        var originalMembers = obj.Item.ToList();
        foreach (var m in obj.Item)
        {
            var id = IdentifierTransformer.TransformIdentifier(m.Key);
            var exp = ExpressionTransformer.TransformExpression(m.Value);
            var member = new ObjectMemberToken(id, exp);
            id.Parent = member;
            id.Slice = SourceSlice.FromSyntaxElementAndCode(m.Key, () => member.Code);
            exp.Parent = member;
            exp.Slice = SourceSlice.FromSyntaxElementAndCode(m.Value, () => member.Code);
            members.Add(member);
        }

        var token = new ObjectExpressionToken(members.ToImmutableList());
        for (int i = 0; i < members.Count; i++)
        {
            var m = members[i];
            var o = originalMembers[i];
            m.Parent = token;
            
            var startCs = o.Key.CodeSpan.Start;
            var endCs = o.Value.CodeSpan.End;
            m.Slice = SourceSlice.FromSourcePosition(startCs, endCs, () => token.Code);
        }

        return token;
    }

    private static TupleExpressionToken TransformTupleExpression(Expression.Expression.Tuple tup)
    {
        var originalElements = tup.Item.ToList();
        var tokenElements = originalElements
            .Select(e => TransformExpression(e))
            .ToImmutableList();
        var token = new TupleExpressionToken(tokenElements);
        for (int i = 0; i < originalElements.Count; i++)
        {
            var original = originalElements[i];
            var transformed = tokenElements[i];
            transformed.Parent = token;
            transformed.Slice = SourceSlice.FromSyntaxElementAndCode(original, () => token.Code);
        }
        return token;
    }
    
    private static ArrayExpressionToken TransformArrayExpression(Expression.Expression.Array arr)
    {
        var originalItems= arr.Item.ToList();
        var tokenItems = originalItems
            .Select(e => TransformExpression(e))
            .ToImmutableList();
        var token = new ArrayExpressionToken(tokenItems);
        for (int i = 0; i < tokenItems.Count; i++)
        {
            var original = originalItems[i];
            var transformed = tokenItems[i];
            transformed.Parent = token;
            transformed.Slice = SourceSlice.FromSyntaxElementAndCode(original, () => token.Code);
        }
        return token;
    }
}
