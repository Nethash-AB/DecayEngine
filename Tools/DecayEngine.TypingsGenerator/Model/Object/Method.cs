using System.Collections.Generic;
using System.Linq;
using System.Text;
using DecayEngine.TypingsGenerator.Model.Value;

namespace DecayEngine.TypingsGenerator.Model.Object
{
    public class Method : Function
    {
        public bool Static { get; }
        public Return Return { get; }

        public Method(string name, string description, bool requiresDeclare, bool requiresAccessModifier,
            bool isStatic, Return ret, params Parameter[] paramList) : base(name, description, requiresDeclare, requiresAccessModifier)
        {
            Static = isStatic;
            Parameters = paramList?.ToList();
            Return = ret ?? new Return(null, "void");
        }

        public override object Clone()
        {
            return new Method(Name, Description, RequiresDeclare, RequiresAccessModifier, Static, Return, Parameters.ToArray());
        }

        public override string ToString(int indentationLevel)
        {
            string indentation = new string(' ', indentationLevel * SpacesPerIndentationLevel);
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(GetDocumentation().ToString(indentationLevel));

            sb.Append($"{indentation}");

            if (Parent is Namespace)
            {
                sb.Append("function ");
            }
            else
            {
                if (RequiresDeclare)
                {
                    sb.Append($"declare function ");
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

            sb.Append($"{Name}(");

            for (int i = 0; i < Parameters.Count; i++)
            {
                sb.Append(Parameters[i]);
                if (i < Parameters.Count - 1)
                {
                    sb.Append(", ");
                }
            }

            sb.Append(")");

            if (Return != null)
            {
                sb.Append(Return);
            }

            sb.Append(";");
            
            return sb.ToString();
        }

        protected override DocumentationBuilder GetDocumentation()
        {
            DocumentationBuilder doc = new DocumentationBuilder();
            doc.Start(Description);

            if (Parameters.Count > 0 || Return.Type != "void")
            {
                doc.AddLine();
            }

            foreach (Parameter param in Parameters)
            {
                doc.AddParameter(param.Name, param.Description);
            }

            if (Return.Type != "void")
            {
                doc.AddReturn(Return.Description);
            }

            doc.End();
            return doc;
        }
    }
}