using System;

namespace DecayEngine.ModuleSDK.Exports.Attributes
{
    [AttributeUsage(AttributeTargets.Parameter, Inherited = false)]
    public class ScriptExportParameterAttribute : ScriptExportAttribute
    {
        public ScriptExportParameterAttribute(string description, Type typeOverride = null)
        {
            Description = description;
            TypeOverride = typeOverride;
        }

        public ScriptExportParameterAttribute(string description, Type[] typeUnionOverride)
        {
            Description = description;
            TypeUnionOverride = typeUnionOverride;
        }
    }
}