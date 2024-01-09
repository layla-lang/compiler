namespace Ares.Compiler.Analysis.Entities.Types;

public interface ICloseable
{
    TypeEntity ProvideTypeParam(TypeArgEntity tp, TypeEntity v);
}
public interface ICloseable<T> where T : TypeEntity
{
    T ProvideTypeParam(TypeArgEntity tp, TypeEntity v);
}