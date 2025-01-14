using System.Collections.Generic;
using System.Linq;
using System.Text;
using DecayEngine.TypingsGenerator.Model.Flags;

namespace DecayEngine.TypingsGenerator.Model.Object
{
    public class Class : DocumentableObject
    {
        public ClassFlags Flags { get; set; }
        public string BaseType { get; set; }
        public List<string> ImplementedInterfaces { get; set; }

        public Class(string name, string description, bool requiresDeclare, ClassFlags flags,
            string baseType, IEnumerable<string> implementedInterfaces = null, params DocumentableObject[] children
        ) : base(name, description, requiresDeclare, false, children)
        {
            Flags = flags;
            BaseType = baseType;
            ImplementedInterfaces = implementedInterfaces?.ToList() ?? new List<string>();
        }

        public override object Clone()
        {
            return new Class(Name, Description, RequiresDeclare, Flags, BaseType, ImplementedInterfaces, Children.ToArray());
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

            if (Flags.HasFlag(ClassFlags.Abstract))
            {
                sb.Append("abstract ");
            }

            sb.Append($"class {Name} ");

            if (!string.IsNullOrEmpty(BaseType))
            {
                sb.Append($"extends {BaseType} ");
            }

            if (ImplementedInterfaces.Count > 0)
            {
                sb.Append("implements ");
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

            if (Flags.HasFlag(ClassFlags.Static))
            {
                doc.AddLine();
                doc.AddLine("@remarks This class is static.");
            }

            if (Flags.HasFlag(ClassFlags.Struct))
            {
                doc.AddLine();
                doc.AddLine("@remarks This class is a struct.");                
            }

            doc.End();
            return doc;
        }
    }
}