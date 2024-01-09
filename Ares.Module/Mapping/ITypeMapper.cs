using Ares.Compiler.Analysis.Entities.Types;
using Ares.Runtime;
using Ares.Runtime.Values;

namespace Ares.Module.Mapping;

public interface ITypeMapper
{
    public Type ClrType { get; }
    public TypeEntity AresType { get; }
    public RuntimeValue Convert(object value, CallSite callSite);
}