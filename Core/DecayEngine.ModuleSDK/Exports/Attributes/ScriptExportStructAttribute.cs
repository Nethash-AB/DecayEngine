using System;

namespace DecayEngine.ModuleSDK.Exports.Attributes
{
    [AttributeUsage(AttributeTargets.Struct, Inherited = false)]
    public class ScriptExportStructAttribute : ScriptExportBlockAttribute
    {
        public ScriptExportStructAttribute(string name, string description, Type nameSpace = null) : base(name, description, nameSpace)
        {
        }
    }
}