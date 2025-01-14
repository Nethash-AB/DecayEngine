using System;

namespace DecayEngine.ModuleSDK.Exports.Attributes
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public class ScriptExportMethodAttribute : ScriptExportAttribute
    {
        public Type NameSpace { get; }
        public Type DelegateType { get; }

        public ScriptExportMethodAttribute(string description, Type nameSpace = null, Type delegateType = null)
        {
            Description = description;
            NameSpace = nameSpace;
            DelegateType = delegateType;
        }
    }
}