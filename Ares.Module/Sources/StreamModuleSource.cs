namespace Ares.Compiler.Module.Sources;

public class StreamModuleSource : ModuleSource
{
    private readonly string name;
    private readonly Stream sourceStream;
    
    public StreamModuleSource(
        string name,
        Stream stream,
        ModuleSourceKind kind) : base(kind)
    {
        this.name = name;
        this.sourceStream = stream;
    }

    public override async Task<ModuleManifest> LoadAsync()
    {
        return new ModuleManifest();
    }
}