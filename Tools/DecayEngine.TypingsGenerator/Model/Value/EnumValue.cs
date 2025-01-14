using System.Text;

namespace DecayEngine.TypingsGenerator.Model.Value
{
    public class EnumValue : ITypedObject
    {
        public string Name { get; }
        public string Value { get; }
        public string Description { get; }
        public string Type { get; set; }

        public EnumValue(string name, string description, string type, string value)
        {
            Name = name;
            Value = value;
            Description = description;
            Type = type;
        }

        public string ToString(int indentationLevel)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(GetDocumentation().ToString(indentationLevel));

            sb.Append(new string(' ', indentationLevel * DocumentableObject.SpacesPerIndentationLevel));
            sb.Append(Name);

            if (!string.IsNullOrEmpty(Value))
            {
                sb.Append($" = {Value}");
            }

            sb.Append(",");

            return sb.ToString();
        }

        public override string ToString()
        {
            return ToString(0);
        }

        private DocumentationBuilder GetDocumentation()
        {
            DocumentationBuilder doc = new DocumentationBuilder();
            doc.Start(Description);
            doc.End();
            return doc;
        }
    }
}