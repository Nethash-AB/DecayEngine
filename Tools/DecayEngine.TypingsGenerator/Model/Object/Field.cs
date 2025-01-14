using System.Text;
using DecayEngine.TypingsGenerator.Model.Value;

namespace DecayEngine.TypingsGenerator.Model.Object
{
    public class Field : DocumentableObject, ITypedObject
    {
        public string Type { get; set; }
        public bool Static { get; }

        public Field(string name, string description, bool requiresDeclare, bool requiresAccessModifier, string type, bool isStatic) : base(name, description, requiresDeclare, requiresAccessModifier)
        {
            Type = type;
            Static = isStatic;
        }

        public override object Clone()
        {
            return new Field(Name, Description, RequiresDeclare, RequiresAccessModifier, Type, Static);
        }

        public override string ToString(int indentationLevel)
        {
            string indentation = new string(' ', indentationLevel * SpacesPerIndentationLevel);
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(GetDocumentation().ToString(indentationLevel));

            sb.Append($"{indentation}");

            if (Parent is Namespace)
            {
                sb.Append("let ");
            }
            else
            {
                if (RequiresDeclare)
                {
                    sb.Append($"declare let ");
                }
                else
                {
                    if (RequiresAccessModifier)
                    {
                        sb.Append($"public ");
                    }
                }

                if (Static)
                {
                    sb.Append("static ");
                }
            }

            sb.Append($"{Name}: {Type};");

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