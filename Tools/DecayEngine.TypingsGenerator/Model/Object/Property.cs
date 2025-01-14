using System.Text;

namespace DecayEngine.TypingsGenerator.Model.Object
{
    public class Property : DocumentableObject, ITypedObject
    {
        public string Type { get; set; }
        public bool Static { get; }
        public bool HasGetter { get; }
        public bool HasSetter { get; }

        public Property(string name, string description, bool requiresDeclare, bool requiresAccessModifier,
            string type, bool isStatic, bool hasGetter, bool hasSetter) : base(name, description, requiresDeclare, requiresAccessModifier)
        {
            Type = type;
            Static = isStatic;
            HasGetter = hasGetter;
            HasSetter = hasSetter;
        }

        public override object Clone()
        {
            return new Property(Name, Description, RequiresDeclare, RequiresAccessModifier, Type, Static, HasGetter, HasSetter);
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
            doc.AddLine();

            if (HasGetter && !HasSetter)
            {
                doc.AddLine("@remarks This property is Read-Only.");
            }
            else if (!HasGetter && HasSetter)
            {
                doc.AddLine("@remarks This property is Write-Only.");
            }
            else if (HasGetter && HasSetter)
            {
                doc.AddLine("@remarks This property is Read/Write.");
            }

            doc.End();
            return doc;
        }
    }
}