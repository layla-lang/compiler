using System.Collections.Immutable;

namespace Ares.Compiler.Tokens;

public enum StatementTokenType
{
    Block,
    VariableDeclaration,
    TypeDeclaration,
    DestructuredVariableDeclaration,
    Return
}

public abstract record StatementToken(StatementTokenType StatementType) : SyntaxToken
{
}


public enum DeclarationTypeType
{
    Inferred,
    TypeDescriptor
}

public abstract record DeclarationTypeToken(DeclarationTypeType DeclarationTypeType);

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
