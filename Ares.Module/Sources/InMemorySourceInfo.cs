namespace Ares.Compiler.Module.Sources;

internal class InMemorySourceInfo : ModuleSourceInfo
{
    private readonly string name;
    private readonly Stream stream;
    
    internal InMemorySourceInfo(string name, Stream stream) : base(ModuleSourceKind.Memory)
    {
        this.name = name;
        this.stream = stream;
    }

    internal override ModuleSource GetModuleSource()
    {
        return new StreamModuleSource(name, stream, ModuleSourceKind.Memory);
    }
}