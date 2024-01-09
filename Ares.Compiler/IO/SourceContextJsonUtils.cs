using System.Reflection;
using Ares.Compiler.Analysis;
using Ares.Compiler.Tables;

namespace Ares.Compiler.IO;

public class SourceContextJsonUtils
{
    public class ScopeMembers
    {
        public Dictionary<string, object> Values { get; } = new Dictionary<string, object>();
        public Dictionary<string, object> Methods { get; } = new Dictionary<string, object>();
        public Dictionary<string, object> Types { get; } = new Dictionary<string, object>();
        public Dictionary<string, object> Operators { get; } = new Dictionary<string, object>();
    }
    
    public record ScopedTableJson(
        string ScopeName,
        Dictionary<Scope, Dictionary<string, object>> ScopedMembers);
    
    public static Dictionary<string, ScopeMembers> ToJson(SourceContext context)
    {
        var vals = ToJsonScopedTable(context.Values);
        var methods = ToJsonScopedTable(context.Methods);
        var types = ToJsonScopedTable(context.Types);
        var operators = ToJsonScopedTable(context.Operators);
        var scopes = vals.ScopedMembers.Keys
            .Concat(methods.ScopedMembers.Keys)
            .Concat(types.ScopedMembers.Keys)
            .Concat(operators.ScopedMembers.Keys)
            .Distinct();
        var mergedDict = new Dictionary<Scope, ScopeMembers>();
        foreach (var s in scopes)
        {
            mergedDict.Add(s, new ScopeMembers());
        }

        foreach (var svp in vals.ScopedMembers)
        {
            foreach (var m in svp.Value)
            {
                mergedDict[svp.Key].Values.Add(m.Key, m.Value);
            }
        }
        foreach (var svp in types.ScopedMembers)
        {
            foreach (var m in svp.Value)
            {
                mergedDict[svp.Key].Types.Add(m.Key, m.Value);
            }
        }
        foreach (var svp in methods.ScopedMembers)
        {
            foreach (var m in svp.Value)
            {
                mergedDict[svp.Key].Methods.Add(m.Key, m.Value);
            }
        }
        foreach (var svp in operators.ScopedMembers)
        {
            foreach (var m in svp.Value)
            {
                mergedDict[svp.Key].Operators.Add(m.Key, m.Value);
            }
        }

        return mergedDict.ToDictionary(
            kvp => kvp.Key.Name,
            kvp => kvp.Value);
    }

    internal static ScopedTableJson ToJsonScopedTable(object scopedTable)
    {
        var valType = scopedTable.GetType();
        var outerScopeField = valType.GetField("knownFromOuterScope", BindingFlags.Instance | BindingFlags.NonPublic)!;
        var currentScopeField = valType.GetField("knownFromCurrentScope", BindingFlags.Instance | BindingFlags.NonPublic)!;
        var scopeField = valType.GetField("myScope", BindingFlags.Instance | BindingFlags.NonPublic)!;
        var outerScope = outerScopeField.GetValue(scopedTable)!;
        var currentScope = currentScopeField.GetValue(scopedTable)!;
        var scope = (Scope)scopeField.GetValue(scopedTable)!;

        var scopedDict = new Dictionary<Scope, Dictionary<string, object>>();
        
        var outerKeys = ToKeys(outerScope);
        foreach (var ok in outerKeys)
        {
            var (valScope, val) = GetValue(outerScope, ok);
            if (!scopedDict.ContainsKey(valScope))
            {
                scopedDict.Add(valScope, new Dictionary<string, object>());
            }
            scopedDict[valScope].Add(ok, val);
        }
        
        var currentKeys = ToKeys(currentScope);
        foreach (var ck in currentKeys)
        {
            var (valScope, val) = GetValue(currentScope, ck);
            if (!scopedDict.ContainsKey(valScope))
            {
                scopedDict.Add(valScope, new Dictionary<string, object>());
            }
            scopedDict[valScope].Add(ck, val);
        }

        return new ScopedTableJson(scope.Name, scopedDict);
    }
    
    private static List<string> ToKeys(object obj)
    {
        var keysProp = obj.GetType().GetProperty("Keys")!;
        var keys = (IEnumerable<string>)keysProp.GetValue(obj)!;
        return keys.ToList();
    }

    private static (Scope, object) GetValue(object obj, string key)
    {
        var objType = obj.GetType();
        var tp = objType.GenericTypeArguments[1];
        var indexer = objType.GetProperty("Item", tp, new Type[]{ typeof(string) })!;
        var scopedResult = indexer.GetValue(obj, new[] { key! })!;
        var srType = scopedResult.GetType();
        var scopeProp = srType.GetProperty("Scope")!;
        var resultProp = srType.GetProperty("Result")!;
        var scope = (Scope)scopeProp.GetValue(scopedResult)!;
        var result = resultProp.GetValue(scopedResult)!;
        return (scope, result);
    }
}