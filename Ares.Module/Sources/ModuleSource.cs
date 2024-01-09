namespace Ares.Module.Sources;

public abstract class ModuleSource
{
    public ModuleSource(ModuleSourceKind kind)
    {
        this.SourceKind = kind;
    }
    
    public ModuleSourceKind SourceKind { get; }

    public abstract Task<ModuleManifest> LoadAsync();

    public virtual ModuleManifest Load() => this.LoadAsync().GetAwaiter().GetResult();
}