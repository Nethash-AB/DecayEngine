using System;

namespace DecayEngine.ModuleSDK.Exports.Attributes
{
    [AttributeUsage(AttributeTargets.Enum, Inherited = false)]
    public class ScriptExportEnumAttribute : ScriptExportBlockAttribute
    {
        public ScriptExportEnumAttribute(string name, string description, Type nameSpace = null) : base(name, description, nameSpace)
        {
        }
    }
}