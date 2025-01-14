using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DecayEngine.TypingsGenerator.Model
{
    public abstract class DocumentableObject : ICloneable
    {
        public string Name { get; protected set; }
        public string FullyQualifiedName => GetFullyQualifiedName();
        public string Description { get; protected set; }
        public bool RequiresDeclare { get; protected set; }
        public bool RequiresAccessModifier { get; set; }
        public List<DocumentableObject> Children { get; }
        public DocumentableObject Parent { get; set; }

        public const int SpacesPerIndentationLevel = 4;

        protected DocumentableObject(string name, string description, bool requiresDeclare, bool requiresAccessModifier, params DocumentableObject[] children)
        {
            Name = name;
            Description = description;
            RequiresDeclare = requiresDeclare;
            RequiresAccessModifier = requiresAccessModifier;
            Children = children.ToList();
        }

        public override string ToString()
        {
            return ToString(0);
        }

        public abstract object Clone();

        private string GetFullyQualifiedName(List<string> names = null)
        {
            bool isInitializer = false;
            if (names == null)
            {
                names = new List<string>();
                isInitializer = true;
            }

            Parent?.GetFullyQualifiedName(names);

            names.Add(Name);

            if (isInitializer)
            {
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < names.Count; i++)
                {
                    string name = names[i];
                    sb.Append(name);

                    if (i < names.Count - 1)
                    {
                        sb.Append(".");
                    }
                }

                return sb.ToString();
            }

            return null;
        }

        public abstract string ToString(int indentationLevel);
        protected abstract DocumentationBuilder GetDocumentation();
    }
}