using System.Collections.Generic;
using System.Text;

namespace Ares.Compiler.TypeScriptGenerator.Codegen
{
    public class TypeHelper
    {
        public class TypeProp
        {
            public TypeProp(string name, string type)
            {
                this.Name = name;
                this.Type = type;
            }
            public string Name { get; }
            public string Type { get; }
            
            public string ToMemberString() => $"{Name}: {Type};";
        }
        private readonly string name;
        private readonly string baseType;
        private readonly List<TypeProp> properties = new List<TypeProp>();

        public TypeHelper(string name, string baseType = null)
        {
            this.name = name;
            this.baseType = baseType;
        }

        public void AddProperty(string name, string type) => properties.Add(new TypeProp(name, type));

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append($"type {name} = ");
            if (baseType != null)
            {
                sb.Append($"{baseType} & ");
            }

            sb.AppendLine("{");
            foreach (var p in properties)
            {
                sb.AppendLine($"  {p.ToMemberString()}");
            }
            sb.AppendLine("}");
            return sb.ToString();
        }
    }
}