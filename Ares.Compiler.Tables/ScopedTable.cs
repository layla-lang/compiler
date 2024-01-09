using System.Collections;
using System.Collections.Immutable;

namespace Ares.Compiler.Tables;

public class ScopedTable<T>
    : IEnumerable<T>, IEnumerable
    where T : ILookupable
{
    private readonly IImmutableDictionary<string, ScopedResult<T>> knownFromOuterScope;
    private readonly IDictionary<string, ScopedResult<T>> knownFromCurrentScope = new Dictionary<string, ScopedResult<T>>();
    private readonly Scope myScope;

    public ScopedTable(Scope scope)
    {
        this.myScope = scope;
        knownFromOuterScope = ImmutableDictionary<string, ScopedResult<T>>.Empty;
    }
    public ScopedTable(Scope scope, IImmutableDictionary<string, T> knownTypes)
    {
        this.myScope = scope;
        this.knownFromOuterScope = knownTypes.ToImmutableDictionary(
            kt => kt.Key,
            kt => new ScopedResult<T>(scope, kt.Value));
    }
    internal ScopedTable(Scope scope, IImmutableDictionary<string, ScopedResult<T>> knownTypes)
    {
        this.myScope = scope;
        this.knownFromOuterScope = knownTypes;
    }

    public void Add(T kt)
    {
        var scoped = new ScopedResult<T>(myScope, kt);
        this.knownFromCurrentScope.Add(kt.Name, scoped);
    }

    public List<ScopedResult<T>> ScanByPrefix(string prefix)
    {
        var items = new List<ScopedResult<T>>();
        foreach (var ent in knownFromCurrentScope)
        {
            if (ent.Key.StartsWith(prefix)) items.Add(ent.Value);
        }
        foreach (var ent in knownFromOuterScope)
        {
            if (ent.Key.StartsWith(prefix)) items.Add(ent.Value);
        }

        return items;
    }
    
    public ScopedResult<T>? this[string name]
    {
        get
        {
            if (knownFromCurrentScope.ContainsKey(name))
            {
                return knownFromCurrentScope[name];
            }
            else if (knownFromOuterScope.ContainsKey(name))
            {
                return knownFromOuterScope[name];
            }

            return null;
        }
    }

    public int Count => this.knownFromCurrentScope.Count + this.knownFromOuterScope.Count;
    public int CurrentScopeCount => this.knownFromCurrentScope.Count;
    public int OuterScopeCount => this.knownFromOuterScope.Count;
    public ScopedTable<T> NewScope(string name) =>
        new ScopedTable<T>(new Scope(name), this.knownFromOuterScope.AddRange(this.knownFromCurrentScope));

    
    private IEnumerable<T> GetEnumerable()
    {
        foreach (var item in this.knownFromCurrentScope)
        {
            yield return item.Value.Result;
        }
        foreach (var item in this.knownFromOuterScope)
        {
            yield return item.Value.Result;
        }
    }

    public IEnumerator GetEnumerator() => GetEnumerable().GetEnumerator();
    IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerable().GetEnumerator();
}