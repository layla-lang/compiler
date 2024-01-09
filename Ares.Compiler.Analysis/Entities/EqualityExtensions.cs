using System.Collections.Immutable;

namespace Ares.Compiler.Analysis.Entities;

public static class EqualityExtensions
{
    public static int GetCombinedHashCode<T>(this IImmutableList<T> source) => 
        source.Aggregate(typeof(T).GetHashCode(), (hash, t) => HashCode.Combine(hash, t));
    public static int GetCombinedHashCode<TKey, TVal>(this IImmutableDictionary<TKey, TVal> source) => 
        source.Aggregate(typeof(KeyValuePair<TKey,TVal>).GetHashCode(), (hash, t) => HashCode.Combine(hash, t));
}