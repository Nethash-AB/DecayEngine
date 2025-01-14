using System.Collections.Generic;
using System.Text;

namespace DecayEngine.TypingsGenerator.Model
{
    public class DocumentationBuilder
    {
        private const string CommentInitializer = "/**";
        private const string CommentLineInitializer = "*";
        private const string CommentFinalizer = "*/";

        private readonly List<string> _lines;

        public DocumentationBuilder()
        {
            _lines = new List<string>();
        }

        public void Start(string description)
        {
            _lines.Add(CommentInitializer);

            foreach (string s in description.Split('\n'))
            {
                AddLine(s.Trim());
            }
        }

        public void AddLine(string line = null)
        {
            if (string.IsNullOrEmpty(line))
            {
                _lines.Add($" { CommentLineInitializer}");
            }
            else
            {
                foreach (string s in line.Split('\n'))
                {
                    _lines.Add($" { CommentLineInitializer} {s.Trim()}");
                }
            }
        }

        public void AddParameter(string name, string description)
        {
            AddLine($"@param {name} - {description}");
        }

        public void AddReturn(string description)
        {
            AddLine($"@returns {description}");
        }

        public void End()
        {
            _lines.Add($" {CommentFinalizer}");
        }

        public void AddIgnore()
        {
            _lines.Add("// @ts-ignore");
        }

        public void Clear()
        {
            _lines.Clear();
        }

        public string ToString(int indentationLevel)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < _lines.Count; i++)
            {
                string s = new string(' ', indentationLevel * DocumentableObject.SpacesPerIndentationLevel) + _lines[i];
                if (i < _lines.Count - 1)
                {
                    sb.AppendLine(s);
                }
                else
                {
                    sb.Append(s);
                }
            }

            return sb.ToString();
        }

        public override string ToString()
        {
            return ToString(0);
        }
    }
}