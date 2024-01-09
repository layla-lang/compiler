using Microsoft.CodeAnalysis;

namespace Ares.Compiler;

public class AresProject
{
    public AresProject(string name)
    {
        this.Name = name;
        this.References = CreateDefaultReferences();
        string ext = ProjectType switch
        {
            ProjectType.Application => "",
            ProjectType.Library => ".dll",
            _ => ""
        };
        this.OutputPath = Path.Join(Environment.CurrentDirectory, $"{name}{ext}");
    }
    
    public string Name { get; set; }
    public ProjectType ProjectType { get; set; } = ProjectType.Application;
    public List<SourceFile> SourceFiles { get; } = new List<SourceFile>();
    public string OutputPath { get; set; } = "";
    public List<MetadataReference> References { get; }
    
    public void AddReferenceFromFile(string file) => References.Add(MetadataReference.CreateFromFile(file));

    private static List<MetadataReference> CreateDefaultReferences() =>
    [
        MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
        MetadataReference.CreateFromFile(typeof(System.Object).Assembly.Location),
    ];
}