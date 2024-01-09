using Ares.Compiler.Parser.Syntax;

namespace Ares.Compiler.Tokens;

using System.Collections.Immutable;

public enum TypeDescriptorType
{
    Never,
    Any,
    TypeParam,
    Id,
    Literal,
    Array,
    Tuple,
    Union,
    Intersection,
    Record,
    Func,
    Indexed
}

public abstract record TypeDescriptorToken(TypeDescriptorType TypeDescriptorType)
    : SyntaxToken()
{
}

public record NeverTypeDescriptorToken() : TypeDescriptorToken(TypeDescriptorType.Never);
public record AnyTypeDescriptorToken() : TypeDescriptorToken(TypeDescriptorType.Any);


public record TypeParamDescriptorToken(string Identifier) : TypeDescriptorToken(TypeDescriptorType.TypeParam);
public record IdTypeDescriptorToken(IdentifierToken Identifier, IImmutableList<TypeDescriptorToken> TypeArguments) : TypeDescriptorToken(TypeDescriptorType.Id);

public record LiteralTypeDescriptorToken(ValueToken Value) : TypeDescriptorToken(TypeDescriptorType.Literal);

public record ArrayTypeDescriptorToken(TypeDescriptorToken ElementType) : TypeDescriptorToken(TypeDescriptorType.Array);
public record TupleTypeDescriptorToken(IImmutableList<TypeDescriptorToken> ElementTypes) : TypeDescriptorToken(TypeDescriptorType.Tuple);

public record UnionTypeDescriptorToken(IImmutableList<TypeDescriptorToken> Types) : TypeDescriptorToken(TypeDescriptorType.Union);

public record IntersectionTypeDescriptorToken(IImmutableList<TypeDescriptorToken> Types)
    : TypeDescriptorToken(TypeDescriptorType.Intersection);

public record RecordTypeDescriptorToken(IImmutableDictionary<IdentifierToken, TypeDescriptorToken> Members)
    : TypeDescriptorToken(TypeDescriptorType.Record);

public record FuncTypeDescriptorToken(
    IImmutableList<TypeParameterToken> TypeParameters,
    IImmutableList<TypeDescriptorToken> Parameters,
    TypeDescriptorToken ReturnType)
    : TypeDescriptorToken(TypeDescriptorType.Func)
{
    
}

public record IndexedTypeDescriptorToken(TypeDescriptorToken IndexedType, TypeDescriptorToken Indexer) : TypeDescriptorToken(TypeDescriptorType.Indexed);