using System.Collections.Immutable;
using Ares.Compiler.Parser;
using Ares.Compiler.Parser.Syntax;
using Ares.Compiler.Transformer;
using Newtonsoft.Json;

namespace Ares.Compiler.Tokens;

public enum StatementTokenType
{
    Block,
    VariableDeclaration,
    TypeDeclaration,
    DestructuredVariableDeclaration,
    Return
}

public abstract record StatementToken([JsonProperty(Order = 1)] StatementTokenType StatementType) : SyntaxToken
{
    public static StatementToken Parse(string code) => TokenParser.ParseToken<StatementToken>(code);
    public static explicit operator StatementToken(Statement.StatementSyntaxElement syntax) =>
        StatementTransformer.TransformStatement(syntax);
}


public enum DeclarationTypeType
{
    Inferred,
    TypeDescriptor
}

public abstract record DeclarationTypeToken([JsonProperty(Order = 1)] DeclarationTypeType DeclarationTypeType);

public record BlockStatementToken(IImmutableList<StatementToken> Statements) : StatementToken(StatementTokenType.Block);

public record InferredDeclaredTypeToken() : DeclarationTypeToken(DeclarationTypeType.Inferred);

public record TypeDescriptorTypeToken(TypeDescriptorToken TypeDescriptor)
    : DeclarationTypeToken(DeclarationTypeType.TypeDescriptor);

public record VariableDeclarationStatementToken(
    SimpleIdentifierToken Identifier,
    DeclarationTypeToken DeclaredType,
    ExpressionToken AssignedValue) : StatementToken(StatementTokenType.VariableDeclaration);

public record TypeDeclarationStatementToken(
    SimpleIdentifierToken Identifier,
    IImmutableList<TypeParameterToken> TypeParameters,
    TypeDescriptorToken AssignedType) : StatementToken(StatementTokenType.TypeDeclaration);

public record DestructuredVariableDeclarationStatementToken(
    IImmutableList<IdentifierToken> Identifiers,
    ExpressionToken Expression) : StatementToken(StatementTokenType.DestructuredVariableDeclaration);

public record ReturnStatementToken(ExpressionToken Expression) : StatementToken(StatementTokenType.Return);
