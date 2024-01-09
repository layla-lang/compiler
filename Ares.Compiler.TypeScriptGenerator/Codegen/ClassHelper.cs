using System.Collections.Generic;
using System.Text;

namespace Ares.Compiler.TypeScriptGenerator.Codegen
{
    public class ClassHelper
    {
        class ClassProp
        {
            public ClassProp(string name, string type)
            {
                this.Name = name;
                this.Type = type;
            }
            public string Name { get; set; }
            public string Type { get; set; }
            public string FieldName => "#" + Name.ToLower();

            public string ToFieldString() => $"{FieldName}: {Type} | undefined;";
            public string ToGetterString() => $"get {Name}(): {Type} {{\n    return this.{FieldName}!;\n  }}";
            public string ToSetterString() => $"set {Name}(_v: {Type}) {{\n    this.{FieldName} = _v;\n  }}";
        }
        private readonly string name;
        private readonly string baseClass;
        private readonly bool isAbstract;
        private readonly List<ClassProp> properties = new List<ClassProp>();

        public ClassHelper(string name, string baseClass = null, bool isAbstract = false)
        {
            this.name = name;
            this.baseClass = baseClass;
            this.isAbstract = isAbstract;
        }

        public void AddProperty(string name, string type)
        {
            properties.Add(new ClassProp(name, type));
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            if (isAbstract) sb.Append("abstract ");
            sb.Append($"class {name}");
            if (baseClass != null)
            {
                sb.Append($" extends {baseClass}");
            }
            sb.AppendLine(" {");
            foreach (var p in properties)
            {
                sb.AppendLine($"  {p.ToFieldString()}");
            }

            sb.AppendLine();
            foreach (var p in properties)
            {
                sb.AppendLine($"  {p.ToGetterString()}");
                sb.AppendLine($"  {p.ToSetterString()}");
            }
            sb.AppendLine("}");
            return sb.ToString();
        }
    }
}