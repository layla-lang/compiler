using System.Collections.Immutable;

namespace Ares.Compiler.Tokens;

public enum MemberTokenType
{
    Func,
    TypeDeclaration,
    Scratch
}
public enum MemberScope
{
    Public,
    Protected,
    Internal,
    Private
}

public enum FunctionReturnTypeType
{
    Inferred,
    TypeDescriptor
}

public abstract record FunctionReturnTypeToken(FunctionReturnTypeType LambdaParameterTypeType);
public record FunctionInferredReturnTypeToken() : FunctionReturnTypeToken(FunctionReturnTypeType.Inferred);
public record FunctionSpecifiedReturnTypeToken(TypeDescriptorToken TypeDescriptor) : FunctionReturnTypeToken(FunctionReturnTypeType.TypeDescriptor);


public abstract record MemberToken(MemberTokenType MemberType) : SyntaxToken
{
}

public record ScratchMemberToken(StatementToken Body) : MemberToken(MemberTokenType.Scratch);
public record FuncMemberToken(
    string Name,
    MemberScope Scope,
    FunctionReturnTypeToken ReturnType,
    IImmutableDictionary<string, TypeDescriptorToken> Parameters,
    IImmutableDictionary<string, TypeParameterToken> TypeParameters,
    IImmutableDictionary<string, IImmutableList<TypeConstraintToken>> TypeConstraints,
    StatementToken Body) : MemberToken(MemberTokenType.Func);

public record TypeDeclarationMemberToken(
    string Name,
    MemberScope Scope,
    IImmutableList<TypeParameterToken> TypeParameters,
    TypeDescriptorToken AssignedType) : MemberToken(MemberTokenType.TypeDeclaration);
