using Ares.Compiler.Analysis.Entities;
using Ares.Compiler.Analysis.Entities.Types;

namespace Ares.Compiler.Analysis.Tables;

public static class TableExtensions
{
    public static TypeEntity? LookupDereferencedType(this ScopedTable<TypeEntity> tbl, string key)
    {
        var m = tbl[key];
        if (!m.HasValue) return null;
        var result = m.Value.Result;
        if (result is AliasedTypeEntity a) return a.ReferencedTypeEntity;
        return result;
    }
}