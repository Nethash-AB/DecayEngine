using System.Linq;
using System.Text;
using DecayEngine.TypingsGenerator.Model.Value;

namespace DecayEngine.TypingsGenerator.Model.Object
{
    public class Constructor : Function
    {
        public Constructor(string description, params Parameter[] paramList) : base("constructor", description, true, false)
        {
            Parameters = paramList.ToList();
        }

        public override object Clone()
        {
            return new Constructor(Description, Parameters.ToArray());
        }

        public override string ToString(int indentationLevel)
        {
            string indentation = new string(' ', indentationLevel * SpacesPerIndentationLevel);
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(GetDocumentation().ToString(indentationLevel));

            sb.Append($"{indentation}");

            sb.Append($"{Name}(");

            for (int i = 0; i < Parameters.Count; i++)
            {
                sb.Append(Parameters[i]);
                if (i < Parameters.Count - 1)
                {
                    sb.Append(", ");
                }
            }

            sb.Append(");");
            return sb.ToString();
        }

        protected override DocumentationBuilder GetDocumentation()
        {
            DocumentationBuilder doc = new DocumentationBuilder();
            doc.Start(Description);
            if (Parameters.Count > 0)
            {
                doc.AddLine();
                foreach (Parameter param in Parameters)
                {
                    doc.AddParameter(param.Name, param.Description);
                }
            }
            doc.End();
            return doc;
        }
    }
}