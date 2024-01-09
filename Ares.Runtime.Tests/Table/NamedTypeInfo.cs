using System.Runtime.InteropServices;
using System.Text;

namespace Ares.Runtime.Tests.Table;

public unsafe struct NamedTypeInfo
{
    public NamedTypeInfo(string name)
    {
        var nameArray = Encoding.UTF8.GetBytes(name);
        fixed (byte* ptr = nameArray)
        {
            int index = 0;
            for (byte* counter = ptr; *counter != 0; counter++)
            {
                TypeEntityId[index++] = *counter;
            }
        }
    }
    public fixed byte TypeEntityId[8];
}