using System;

namespace DecayEngine.ModuleSDK.Exports.Attributes
{
    [AttributeUsage(AttributeTargets.ReturnValue, Inherited = false)]
    public class ScriptExportReturnAttribute : ScriptExportAttribute
    {
        public ScriptExportReturnAttribute(string description, Type typeOverride = null)
        {
            Description = description;
            TypeOverride = typeOverride;
        }

        public ScriptExportReturnAttribute(string description, Type[] typeUnionOverride)
        {
            Description = description;
            TypeUnionOverride = typeUnionOverride;
        }
    }
}