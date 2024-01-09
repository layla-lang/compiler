using Ares.Compiler.Parser;
using Ares.Compiler.Tokens;

namespace Ares.Compiler.Tests.Parser;

public class TokenParserTest
{
    [Fact]
    public void ParseTokenParsesExpressionTokens()
    {
        var constToken = TokenParser.ParseToken<ConstantExpressionToken>("4");
        Assert.IsType<ConstantExpressionToken>(constToken);
        Assert.Equal(4, ((IntLiteralValueToken)constToken.Value).Value);
        Assert.Equal("4", constToken.SyntaxText);
        Assert.Equal("4", constToken.Code.Code);
    }
    [Fact]
    public void ParseTokenParsesIdentifierTokens()
    {
        var simpleToken = TokenParser.ParseToken<SimpleIdentifierToken>("x");
        Assert.IsType<SimpleIdentifierToken>(simpleToken);
    }
    [Fact]
    public void ParseTokenParsesValueTokens()
    {
        var intToken = TokenParser.ParseToken<IntLiteralValueToken>("4");
        Assert.IsType<IntLiteralValueToken>(intToken);
    }
    [Fact]
    public void ParseTokenParsesTypeDesTokens()
    {
        var unionToken = TokenParser.ParseToken<UnionTypeDescriptorToken>("Int | String");
        Assert.IsType<UnionTypeDescriptorToken>(unionToken);
    }
    [Fact]
    public void ParseTokenParsesTypeParamTokens()
    {
        var tToken = TokenParser.ParseToken<TypeParameterToken>("'T");
        Assert.IsType<TypeParameterToken>(tToken);
    }
    
    [Fact]
    public void ParseTokenParsesStatementTokens()
    {
        var varDeclToken = TokenParser.ParseToken<VariableDeclarationStatementToken>("Int x = 4;");
        Assert.IsType<VariableDeclarationStatementToken>(varDeclToken);
    }
    
    [Fact]
    public void IdentifiesClosestTypeWithOperator()
    {
        Assert.Equal(typeof(ExpressionToken), TokenParser.GetTypeWithOperator(typeof(ConstantExpressionToken)));
        Assert.Equal(typeof(IdentifierToken), TokenParser.GetTypeWithOperator(typeof(SimpleIdentifierToken)));
        Assert.Equal(typeof(ValueToken), TokenParser.GetTypeWithOperator(typeof(IntLiteralValueToken)));
        Assert.Equal(typeof(TypeDescriptorToken), TokenParser.GetTypeWithOperator(typeof(UnionTypeDescriptorToken)));
        Assert.Equal(typeof(TypeParameterToken), TokenParser.GetTypeWithOperator(typeof(TypeParameterToken)));
        Assert.Equal(typeof(StatementToken), TokenParser.GetTypeWithOperator(typeof(VariableDeclarationStatementToken)));
    }
}