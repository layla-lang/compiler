using Ares.Runtime;
using Ares.Runtime.Values;

namespace Ares.Module.Mapping;

public delegate RuntimeValue ClrValueConverter<
    TClrType
>(
    TClrType clrValue,
    CallSite callSite);