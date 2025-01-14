using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DecayEngine.TypingsGenerator.Model.Object
{
    public class Interface : DocumentableObject
    {
        public List<string> ImplementedInterfaces { get; }

        public Interface(string name, string description, bool requiresDeclare,
            IEnumerable<string> implementedInterfaces = null, params DocumentableObject[] children
        ) : base(name, description, requiresDeclare, false, children)
        {
            ImplementedInterfaces = implementedInterfaces?.ToList() ?? new List<string>();
        }

        public override object Clone()
        {
            return new Interface(Name, Description, RequiresDeclare, ImplementedInterfaces, Children.ToArray());
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

            sb.Append($"interface {Name} ");

            if (ImplementedInterfaces.Count > 0)
            {
                sb.Append("extends ");
                for (int i = 0; i < ImplementedInterfaces.Count; i++)
                {
                    sb.Append(ImplementedInterfaces[i]);
                    sb.Append(i < ImplementedInterfaces.Count - 1 ? ", " : " ");
                }
            }

            sb.Append("{\n");

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
            return doc;
        }
    }
}