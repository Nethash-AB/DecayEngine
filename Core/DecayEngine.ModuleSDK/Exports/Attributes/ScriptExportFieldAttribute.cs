using System;

namespace DecayEngine.ModuleSDK.Exports.Attributes
{
    [AttributeUsage(AttributeTargets.Field, Inherited = false)]
    public class ScriptExportFieldAttribute : ScriptExportAttribute
    {
        public Type NameSpace { get; }

        public ScriptExportFieldAttribute(string description, Type typeOverride = null, Type nameSpace = null)
        {
            Description = description;
            TypeOverride = typeOverride;
            NameSpace = nameSpace;
        }

        public ScriptExportFieldAttribute(string description, Type[] typeUnionOverride, Type nameSpace = null)
        {
            Description = description;
            TypeUnionOverride = typeUnionOverride;
            NameSpace = nameSpace;
        }
    }
}