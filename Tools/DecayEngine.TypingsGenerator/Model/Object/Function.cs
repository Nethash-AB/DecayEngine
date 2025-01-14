using System.Collections.Generic;
using DecayEngine.TypingsGenerator.Model.Value;

namespace DecayEngine.TypingsGenerator.Model.Object
{
    public abstract class Function : DocumentableObject
    {
        public List<Parameter> Parameters { get; protected set; }

        protected Function(string name, string description, bool requiresDeclare, bool requiresAccessModifier)
            : base(name, description, requiresDeclare, requiresAccessModifier)
        {
        }
    }
}