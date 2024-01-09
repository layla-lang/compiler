namespace Ares.Module;

[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false)]
public sealed class InlineModuleAttribute : Attribute
{
    private readonly string moduleName;
    
    public InlineModuleAttribute(string moduleName)
    {
        this.moduleName = moduleName;
    }

    public string ModuleName => moduleName;
}