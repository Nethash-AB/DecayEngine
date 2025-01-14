using System;

namespace DecayEngine.ModuleSDK.Exports.Attributes
{
    public abstract class ScriptExportAttribute : Attribute
    {
        public string Description { get; protected set; }
        public Type TypeOverride { get; protected set; }
        public Type[] TypeUnionOverride { get; protected set; }
    }
}