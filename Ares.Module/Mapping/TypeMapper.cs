using Ares.Compiler.Analysis.Entities.Types;
using Ares.Runtime;
using Ares.Runtime.Values;

namespace Ares.Module.Mapping;

public record TypeMapper<TClrType>(
    TypeEntity AresType,
    ClrValueConverter<TClrType> Converter)
{
    public Type ClrType => typeof(TClrType);
    public RuntimeValue Convert(TClrType value, CallSite callSite) => Converter(value, callSite);
}