using System.Collections.Immutable;
using System.Reflection;
using Ares.Compiler.Checkpoints;
using Ares.Compiler.Parser.Syntax;
using Ares.Compiler.Tokens;

namespace Ares.Compiler.Parser;

public static class TokenParser
{
    private static readonly IImmutableDictionary<Type, Func<string, object>> expressionFactoryDict;

    static TokenParser()
    {
        expressionFactoryDict = typeof(CodeParser)
            .GetMethods(BindingFlags.Public | BindingFlags.Static)
            .ToImmutableDictionary(
                m => m.ReturnType,
                (m) =>
                {
                    Func<string, object> tokenFactory = (string code) => m.Invoke(null, new object[] { code })!;
                    return tokenFactory;
                });
    }

    public static TokenType ParseToken<TokenType>(string code) where TokenType : SyntaxToken
    {
        var tt = typeof(TokenType);
        var tokenTypeWithOperator = GetTypeWithOperator(tt);
        var castOperator = GetOperator(tokenTypeWithOperator);
        var castParameterType = castOperator.GetParameters().First().ParameterType;
        var factory = expressionFactoryDict
            .First(kvp => castParameterType == kvp.Key)
            .Value;
        var element = (Common.SyntaxElement)factory(code);
        var castMethod = tokenTypeWithOperator.GetMethod("op_Explicit", BindingFlags.Public | BindingFlags.Static,
            new Type[] { element.GetType() })!;
        var token = (SyntaxToken)castMethod.Invoke(null, new[] { element })!;
        var tree = new SyntaxTree(new SourceCode("module", code));
        token.Parent = tree;
        token.Slice = SourceSlice.FromSyntaxElementAndCode(element, () => tree.Code);
        return (TokenType)token;
    }

    internal static Type GetTypeWithOperator(Type specificType)
    {
        var st = typeof(SyntaxToken);
        Type t = specificType;
        while (t.BaseType != st)
        {
            t = t.BaseType!;
        }

        return t;
    }

    internal static MethodInfo GetOperator(Type tokenType) => tokenType
        .GetMethods(BindingFlags.Public | BindingFlags.Static)
        .Where(m => m.Name == "op_Explicit")
        .Where(m => m.ReturnType == tokenType)
        .First(m =>
        {
            var ps = m.GetParameters();
            if (ps.Length != 1) return false;
            var p = ps[0].ParameterType;
            if (!typeof(Common.SyntaxElement).IsAssignableFrom(p))
            {
                return false;
            }

            return true;
        });
}