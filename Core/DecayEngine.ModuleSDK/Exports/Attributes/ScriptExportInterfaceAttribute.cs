using System;

namespace DecayEngine.ModuleSDK.Exports.Attributes
{
    [AttributeUsage(AttributeTargets.Interface, Inherited = false)]
    public class ScriptExportInterfaceAttribute : ScriptExportBlockAttribute
    {
        public ScriptExportInterfaceAttribute(string name, string description, Type nameSpace = null) : base(name, description, nameSpace)
        {
        }
    }
}