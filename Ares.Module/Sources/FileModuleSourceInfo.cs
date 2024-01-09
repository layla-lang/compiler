namespace Ares.Compiler.Module.Sources;

internal sealed class FileModuleSourceInfo : ModuleSourceInfo
{
    private readonly string file;
    
    internal FileModuleSourceInfo(string file) : base(ModuleSourceKind.FileSystem)
    {
        this.file = file;
    }
    
    internal override ModuleSource GetModuleSource()
    {
        var fileName = Path.GetFileName(this.file);
        using var stream = File.OpenRead(file);
        return new StreamModuleSource(fileName, stream, ModuleSourceKind.FileSystem);
    }
}