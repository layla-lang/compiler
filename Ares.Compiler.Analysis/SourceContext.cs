using Ares.Compiler.Analysis.Entities;
using Ares.Compiler.Analysis.Entities.Types;
using Ares.Compiler.Analysis.Taxonomy;
using Ares.Compiler.Tables;

namespace Ares.Compiler.Analysis;

public record SourceContext
{
    public Scope Scope { get; private init; }
    public ScopedTable<TypeEntity> Types { get; private init; }
    public ScopedTable<KnownValue> Values { get; private init; }
    public ScopedTable<KnownOperator> Operators { get; private init; }
    public ScopedTable<KnownMethod> Methods { get; private init; }
    public Universe Universe { get; private init; }

    public SourceContext NewScoped(string name) => new SourceContext()
    {
        Scope = new Scope(name),
        Types = Types.NewScope(name),
        Values = Values.NewScope(name),
        Operators = Operators.NewScope(name),
        Methods = Methods.NewScope(name),
        Universe = Universe.NewScoped(name),
    };
    
    public SourceContext()
    {
        Scope = Scope.BuiltIns;
        Types = new ScopedTable<TypeEntity>(Scope.BuiltIns, TypeEntity.Primitives);
        Values = new ScopedTable<KnownValue>(Scope.BuiltIns);
        Operators = new ScopedTable<KnownOperator>(Scope.BuiltIns, KnownOperator.BuiltinOperators);
        Methods = new ScopedTable<KnownMethod>(Scope.BuiltIns);
        Universe = new Universe(Scope.BuiltIns);
    }
}