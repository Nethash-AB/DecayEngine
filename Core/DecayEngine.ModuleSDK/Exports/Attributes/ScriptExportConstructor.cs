using System;

namespace DecayEngine.ModuleSDK.Exports.Attributes
{
    [AttributeUsage(AttributeTargets.Constructor, Inherited = false)]
    public class ScriptExportConstructor : ScriptExportAttribute
    {
        public ScriptExportConstructor(string description = "Constructor.")
        {
            Description = description;
        }
    }
}