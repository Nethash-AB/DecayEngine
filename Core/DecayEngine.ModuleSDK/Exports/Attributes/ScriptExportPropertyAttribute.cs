using System;

namespace DecayEngine.ModuleSDK.Exports.Attributes
{
    [AttributeUsage(AttributeTargets.Property, Inherited = false)]
    public class ScriptExportPropertyAttribute : ScriptExportAttribute
    {
        public Type NameSpace { get; }

        public ScriptExportPropertyAttribute(string description, Type typeOverride = null, Type nameSpace = null)
        {
            Description = description;
            TypeOverride = typeOverride;
            NameSpace = nameSpace;
        }

        public ScriptExportPropertyAttribute(string description, Type[] typeUnionOverride, Type nameSpace = null)
        {
            Description = description;
            TypeUnionOverride = typeUnionOverride;
            NameSpace = nameSpace;
        }
    }
}