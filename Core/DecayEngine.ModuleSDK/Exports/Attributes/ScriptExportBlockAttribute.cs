using System;

namespace DecayEngine.ModuleSDK.Exports.Attributes
{
    public abstract class ScriptExportBlockAttribute : ScriptExportAttribute
    {
        public string Name { get; }
        public Type NameSpace { get; }

        protected ScriptExportBlockAttribute(string name, string description, Type nameSpace = null)
        {
            Name = name;
            Description = description;
            NameSpace = nameSpace;
        }
    }
}