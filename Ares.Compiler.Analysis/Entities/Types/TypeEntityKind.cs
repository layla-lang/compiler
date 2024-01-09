namespace Ares.Compiler.Analysis.Entities.Types;

public enum TypeEntityKind
{
    Any,
    Never,
    TypeArg,
    Aliased,
    Array,
    Primitive,
    Literal,
    Tuple,
    Record,
    Union,
    Intersection,
    Func
}