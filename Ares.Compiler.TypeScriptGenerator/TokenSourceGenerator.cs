using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Ares.Compiler.TypeScriptGenerator.Codegen;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Ares.Compiler.TypeScriptGenerator
{
    [Generator]
    public class TokenSourceGenerator : ISourceGenerator
    {
        public void Execute(GeneratorExecutionContext context)
        {
            TokenSyntaxReceiver syntaxReceiver = (TokenSyntaxReceiver)context.SyntaxReceiver;
            var recordDict = syntaxReceiver.Records.ToDictionary(
                rds => rds.Identifier.Text,
                rds => rds);
            var enumDict = syntaxReceiver.Enums.ToDictionary(
                rds => rds.Identifier.Text,
                rds => rds);
            var directDescendents = syntaxReceiver.Records
                .Where(rds => DoesExtend(rds, "SyntaxToken"))
                .ToList();
            
            string path = Path.Combine(syntaxReceiver.Directory, "tokens.ts");
            
            if (File.Exists(path))
            {
                File.Delete(path);
            }

            return;
            using (var fs = File.OpenWrite(path))
            using (var writer = new StreamWriter(fs))
            {
                var gen = new GenHelper();
                //writer.WriteLine("import { z } from \"zod\";");
                WriteSharedTypes(writer);
                foreach (var r in directDescendents)
                {
                    var rch = new TypeHelper(r.Identifier.Text, "SyntaxToken");
                    List<ParameterSyntax> parameters = r?.ParameterList?.Parameters.ToList() ?? new List<ParameterSyntax>();
                    foreach (var p in parameters)
                    {
                        rch.AddProperty(p.Identifier.Text, p.Type.GetText().ToString().Trim());
                        if (enumDict.ContainsKey(p.Type.GetText().ToString().Trim()))
                        {
                            gen.GenerateEnum(enumDict[p.Type.GetText().ToString().Trim()]);
                        }
                    }

                    var properties = r.Members
                        .OfType<PropertyDeclarationSyntax>()
                        .ToList();
                    foreach (var m in properties)
                    {
                        rch.AddProperty(m.Identifier.Text, m.Type.GetText().ToString().Trim());
                        if (enumDict.ContainsKey(m.Type.GetText().ToString().Trim()))
                        {
                            gen.GenerateEnum(enumDict[m.Type.GetText().ToString().Trim()]);
                        }
                    }
                    gen.AddType(r.Identifier.Text, rch);
                }
                writer.WriteLine(gen.ToString());
            }
            Console.WriteLine($"Wrote to: {path}");
        }

        public void Initialize(GeneratorInitializationContext context)
        {
            context.RegisterForSyntaxNotifications(() => new TokenSyntaxReceiver());
        }

        private void WriteSharedTypes(StreamWriter writer)
        {
            var ch = new ClassHelper("SyntaxToken", isAbstract: true);
            ch.AddProperty("Slice", "SourceSlice");
            ch.AddProperty("SyntaxText", "string");
            writer.WriteLine(@"
type SourcePosition = {
    Row: number;
    Col: number;
    Index: number;
}
type SourceSlice = {
  Start: SourcePosition;
  End: SourcePosition;
  StartIndex: number;
  EndIndex: number;
  Length: number;
  Value: string;
};");
            writer.WriteLine(ch.ToString());
        }
        
        private bool DoesExtend(RecordDeclarationSyntax rds, string extendType)
        {
            if (rds.BaseList == null) return false;
            var pc = rds.BaseList.ChildNodes()
                .OfType<PrimaryConstructorBaseTypeSyntax>()
                .Any(r => r.Type.GetText().ToString().Trim() == extendType);
            var sbt = rds.BaseList.ChildNodes()
                .OfType<SimpleBaseTypeSyntax>()
                .Any(r => r.Type.GetText().ToString().Trim() == extendType);
            return pc || sbt;
        }
    }
    
    public class TokenSyntaxReceiver : ISyntaxReceiver
    {
        public List<RecordDeclarationSyntax> Records { get; } = new List<RecordDeclarationSyntax>();
        public List<EnumDeclarationSyntax> Enums { get; } = new List<EnumDeclarationSyntax>();
        private string dir = null;
        public string Directory => dir;

        public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
        {
            // Business logic to decide what we're interested in goes here
            if (syntaxNode is RecordDeclarationSyntax rds)
            {
                Records.Add(rds);
            }
            else if (syntaxNode is EnumDeclarationSyntax eds)
            {
                Enums.Add(eds);
            }

            if (dir == null)
            {

                var nodeDir = Path.GetDirectoryName(syntaxNode.SyntaxTree.FilePath);
                while (!File.Exists(Path.Combine(nodeDir, "Ares.Compiler.csproj")))
                {
                    nodeDir = Path.GetDirectoryName(nodeDir);
                }

                dir = Path.GetFullPath(Path.Combine(nodeDir, @"../ares-playground/src/codegen"));
            }
        }
    }
}