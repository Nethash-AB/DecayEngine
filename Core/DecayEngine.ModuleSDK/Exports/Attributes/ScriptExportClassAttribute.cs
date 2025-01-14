using System;

namespace DecayEngine.ModuleSDK.Exports.Attributes
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class ScriptExportClassAttribute : ScriptExportBlockAttribute
    {
        public ScriptExportClassAttribute(string name, string description, Type nameSpace = null) : base(name, description, nameSpace)
        {
        }
    }
}