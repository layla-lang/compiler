using System.Collections.Immutable;
using Ares.Compiler.Parser;
using Ares.Compiler.Parser.Syntax;
using Ares.Compiler.Transformer;
using Newtonsoft.Json;

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

public abstract record FunctionReturnTypeToken([JsonProperty(Order = 1)] FunctionReturnTypeType LambdaParameterTypeType);
public record FunctionInferredReturnTypeToken() : FunctionReturnTypeToken(FunctionReturnTypeType.Inferred);
public record FunctionSpecifiedReturnTypeToken(TypeDescriptorToken TypeDescriptor) : FunctionReturnTypeToken(FunctionReturnTypeType.TypeDescriptor);


public abstract record MemberToken([JsonProperty(Order = 1)] MemberTokenType MemberType) : SyntaxToken
{

    public static MemberToken Parse(string code) => TokenParser.ParseToken<MemberToken>(code);
    public static explicit operator MemberToken(Member.MemberSyntaxElement syntax) =>
        MemberTransformer.TransformMember(syntax);
    
    
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
