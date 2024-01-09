using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Ares.Compiler.TypeScriptGenerator.Codegen
{
    public class EnumHelper
    {
        private readonly EnumDeclarationSyntax enumDecl;

        public EnumHelper(EnumDeclarationSyntax enumDecl)
        {
            this.enumDecl = enumDecl;
        }

        public override string ToString() => $"type {enumDecl.Identifier.Text} = " +
            string.Join(" | ", enumDecl.Members
                .Select(m => "\"" + m.Identifier.Text + "\""));
    }
}