using System.Text;
using Ares.Compiler.Parser;
using Ares.Compiler.Transformer;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;

namespace Ares.Compiler;

public class AresCompiler
{
    private readonly CompilerOptions options;

    public AresCompiler(CompilerOptions options)
    {
        this.options = options;
    }
    
    public CompilationResult<string> CompileProject(AresProject project)
    {
        var csharpCompilerOptions = CreateCompilationOptionsFromProject(this.options, project);
        var compilation = CSharpCompilation.Create("Executor.cs")
            .WithOptions(csharpCompilerOptions)
            .WithReferences(project.References);
        foreach (var src in project.SourceFiles)
        {
            //compilation = compilation.AddSyntaxTrees(ToTree(project, src.Text));
        }

        using var fs = File.OpenWrite(project.OutputPath);
            // Actually compile the code
        var compilationResult = compilation.Emit(fs);
        if (!compilationResult.Success)
        {
            var sb = new StringBuilder();
            foreach (var diag in compilationResult.Diagnostics)
            {
                sb.AppendLine(diag.ToString());
            }
            string errorMessage = sb.ToString();
            throw new Exception(errorMessage);
        }

        return new CompilationResult<string>();
    }

    internal static CSharpCompilationOptions CreateCompilationOptionsFromProject(CompilerOptions options, AresProject project)
    {
        var ok = project.ProjectType == ProjectType.Application
            ? OutputKind.ConsoleApplication
            : OutputKind.DynamicallyLinkedLibrary;
        var optimizationLevel = options.DebugMode
            ? OptimizationLevel.Debug
            : OptimizationLevel.Release;
        return new CSharpCompilationOptions(ok, optimizationLevel: optimizationLevel);
    }
}