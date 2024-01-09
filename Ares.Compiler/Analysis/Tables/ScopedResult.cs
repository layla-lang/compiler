namespace Ares.Compiler.Analysis.Tables;

public record struct ScopedResult<TResult>(Scope Scope, TResult Result) where TResult : ILookupable;