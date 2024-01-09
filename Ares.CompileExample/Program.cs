using Ares.Compiler;

string src = """
             int x = 4;
             string y = "hello";
             x =  x + 5;
             """;

var proj = new AresProject("Example");
proj.SourceFiles.Add(SourceFile.FromString(src));
var compiler = new AresCompiler(new CompilerOptions()
{
    DebugMode = false
});
compiler.CompileProject(proj);