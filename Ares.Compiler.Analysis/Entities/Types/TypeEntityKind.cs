namespace Ares.Compiler.Analysis.Entities.Types;

public enum TypeEntityKind
{
    Any = 0,
    Never = 1,
    TypeArg = 2,
    Aliased = 3,
    Array = 4,
    Primitive = 5,
    Literal = 6,
    Tuple = 7,
    Record = 8,
    Union = 9,
    Intersection = 10,
    Func = 11,
}