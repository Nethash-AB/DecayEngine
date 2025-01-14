using System.Collections.Generic;
using System.Linq;
using System.Text;
using DecayEngine.TypingsGenerator.Model.Value;

namespace DecayEngine.TypingsGenerator.Model.Object
{
    public class Enum : DocumentableObject, ITypedObject
    {
        public string Type { get; set; }
        public List<EnumValue> Values { get; }

        public Enum(string name, string description, bool requiresDeclare, string type, params EnumValue[] values) : base(name, description, requiresDeclare, false)
        {
            Type = type;
            Values = values.ToList();
        }

        public override object Clone()
        {
            return new Enum(Name, Description, RequiresDeclare, Type, Values.ToArray());
        }

        public override string ToString(int indentationLevel)
        {
            string indentation = new string(' ', indentationLevel * SpacesPerIndentationLevel);
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(GetDocumentation().ToString(indentationLevel));

            sb.Append($"{indentation}");

            if (RequiresDeclare)
            {
                sb.Append($"declare ");
            }

            sb.Append($"enum {Name} {{\n");
            for (int i = 0; i < Values.Count; i++)
            {
                sb.AppendLine(Values[i].ToString(indentationLevel + 1));
                if (i < Values.Count - 1)
                {
                    sb.AppendLine();
                }
            }
            sb.Append($"{indentation}}}");

            return sb.ToString();
        }

        protected override DocumentationBuilder GetDocumentation()
        {
            DocumentationBuilder doc = new DocumentationBuilder();
            doc.Start(Description);
            doc.End();
            return doc;
        }
    }
}