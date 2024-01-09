using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Ares.Compiler.TypeScriptGenerator.Codegen
{
    public class GenHelper
    {
        private readonly Dictionary<string, EnumHelper> enums = new Dictionary<string, EnumHelper>();
        private readonly Dictionary<string, TypeHelper> types = new Dictionary<string, TypeHelper>();

        public void GenerateEnum(EnumDeclarationSyntax e)
        {
            if (!enums.ContainsKey(e.Identifier.Text))
            {
                enums.Add(e.Identifier.Text, new EnumHelper(e));
            }
        }

        public void AddType(string name, TypeHelper t)
        {
            if (!types.ContainsKey(name))
            {
                types.Add(name, t);
            }
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            foreach (var e in enums.Values)
            {
                sb.AppendLine(e.ToString());
            }
            foreach (var t in types.Values)
            {
                sb.AppendLine(t.ToString());
            }

            return sb.ToString();
        }
    }
}