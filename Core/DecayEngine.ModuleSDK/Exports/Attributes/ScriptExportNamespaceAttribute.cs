using System;

namespace DecayEngine.ModuleSDK.Exports.Attributes
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class ScriptExportNamespaceAttribute : ScriptExportBlockAttribute
    {
        public bool OverrideExisting { get; }
        public ScriptExportNamespaceAttribute(string name, string description, bool overrideExisting = false, Type nameSpace = null) : base(name, description, nameSpace)
        {
            OverrideExisting = overrideExisting;
        }
    }
}