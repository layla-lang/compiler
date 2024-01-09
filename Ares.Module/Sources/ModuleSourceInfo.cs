namespace Ares.Compiler.Module.Sources;

public abstract class ModuleSourceInfo
{
    internal ModuleSourceInfo(ModuleSourceKind kind)
    {
        this.SourceKind = kind;
    }
    
    internal ModuleSourceKind SourceKind { get; }

    internal abstract ModuleSource GetModuleSource();

    public static ModuleSourceInfo FromFile(string fileName) =>
        new FileModuleSourceInfo(fileName);
    public static ModuleSourceInfo FromStream(string moduleName, Stream stream) =>
        new InMemorySourceInfo(moduleName, stream);
}