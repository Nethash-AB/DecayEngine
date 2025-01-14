using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DecayEngine.TypingsGenerator.Model.Object
{
    public class Namespace : DocumentableObject
    {
        public bool OverrideExisting { get; }

        public Namespace(string name, string description, bool requiresDeclare, bool overrideExisting, params DocumentableObject[] children)
            : base(name, description, requiresDeclare, false, children)
        {
            OverrideExisting = overrideExisting;
        }

        public override object Clone()
        {
            return new Namespace(Name, Description, RequiresDeclare, OverrideExisting, Children.ToArray());
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

            sb.Append($"namespace {Name} {{\n");

            List<DocumentableObject> orderedChildren = Children.OrderByDescending(child => child, new DocumentableObjectTypeComparer()).ToList();
            for (int i = 0; i < orderedChildren.Count; i++)
            {
                sb.AppendLine(orderedChildren[i].ToString(indentationLevel + 1));
                if (i < orderedChildren.Count - 1)
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

            if (OverrideExisting)
            {
                doc.AddIgnore();
            }

            return doc;
        }
    }
}