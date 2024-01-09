using System.Runtime.InteropServices;
using Ares.Runtime.Table;

namespace Ares.Runtime.Tests.Table;

public class NamedTypeLeveler : ILevelChanger<NamedType, NamedTypeInfo>
{
    public NamedTypeInfo Downlevel(NamedType highLevel) => new NamedTypeInfo(highLevel.TypeName);

    public unsafe NamedType Uplevel(NamedTypeInfo lowLevel)
    {
        var str = Marshal.PtrToStringUTF8(new IntPtr(lowLevel.TypeEntityId));
        return new NamedType(str);
    }
}