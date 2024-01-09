using System.Collections.Immutable;
using Ares.Compiler.Analysis.Entities.Types;
using Ares.Compiler.Analysis.Tables;

namespace Ares.Compiler.Analysis.Entities;

public abstract record KnownOperator(TypeEntity ResultType) : ILookupable
{
    public abstract string Name { get; }

    public static KnownOperator IntegerAddition =       new KnownBinaryOperator("+", TypeEntity.Int, TypeEntity.Int, TypeEntity.Int);
    public static KnownOperator IntegerSubtraction =    new KnownBinaryOperator("-", TypeEntity.Int, TypeEntity.Int, TypeEntity.Int);
    public static KnownOperator IntegerMultiplication = new KnownBinaryOperator("*", TypeEntity.Int, TypeEntity.Int, TypeEntity.Int);
    public static KnownOperator IntegerDivision =       new KnownBinaryOperator("/", TypeEntity.Int, TypeEntity.Int, TypeEntity.Int);
    public static KnownOperator IntegerModulus =        new KnownBinaryOperator("%", TypeEntity.Int, TypeEntity.Int, TypeEntity.Int);
    public static KnownOperator IntegerPow =            new KnownBinaryOperator("^", TypeEntity.Int, TypeEntity.Int, TypeEntity.Int);
    public static KnownOperator IntegerGt =             KnownBinaryOperator.ComparisonOperator(">" , TypeEntity.Int);
    public static KnownOperator IntegerGte =            KnownBinaryOperator.ComparisonOperator(">=", TypeEntity.Int);
    public static KnownOperator IntegerLt =             KnownBinaryOperator.ComparisonOperator("<" , TypeEntity.Int);
    public static KnownOperator IntegerLte =            KnownBinaryOperator.ComparisonOperator("<=", TypeEntity.Int);
    public static KnownOperator IntegerEq  =            KnownBinaryOperator.ComparisonOperator("==", TypeEntity.Int);
    public static KnownOperator IntegerNotEq  =         KnownBinaryOperator.ComparisonOperator("!=", TypeEntity.Int);

    public static IImmutableDictionary<string, KnownOperator> BuiltinOperators = ImmutableList.Create<KnownOperator>()
        .AddRange(new[]
        {
            IntegerAddition,
            IntegerSubtraction,
            IntegerMultiplication,
            IntegerDivision,
            IntegerModulus,
            IntegerPow,
            IntegerGt,
            IntegerGte,
            IntegerLt,
            IntegerLte,
            IntegerEq,
            IntegerNotEq
        }).ToImmutableDictionary(k => k.Name, k => k);

    private static string ResolveType(TypeEntity te)
    {
        if (te is IAliased a)
        {
            return a.AliasedTo;
        }

        return te.Name;
    }
}

public record KnownUnaryOperator(string Symbol, TypeEntity OperandType, TypeEntity ResultType) : KnownOperator(ResultType)
{
    public override string Name => GetName(Symbol, OperandType);

    public static string GetName(string Symbol, TypeEntity OperandType) =>
        $"\"{Symbol}\"1`[{OperandType.ResolvedName}]";
}
public record KnownBinaryOperator(string Symbol, TypeEntity Left, TypeEntity Right, TypeEntity ResultType) : KnownOperator(ResultType)
{
    public override string Name => GetName(Symbol, Left, Right);

    public static KnownBinaryOperator ComparisonOperator(string Symbol, TypeEntity Item) =>
        new KnownBinaryOperator(Symbol, Item, Item, TypeEntity.Bool);
    
    public static string GetName(string Symbol, TypeEntity Left, TypeEntity Right) =>
        $"\"{Symbol}\"2`[{Left.ResolvedName},{Right.ResolvedName}]";
}