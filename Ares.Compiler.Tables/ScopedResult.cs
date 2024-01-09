namespace Ares.Compiler.Tables;

public record struct ScopedResult<TResult>(Scope Scope, TResult Result) where TResult : ILookupable;