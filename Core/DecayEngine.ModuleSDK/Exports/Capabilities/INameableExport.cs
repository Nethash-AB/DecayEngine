using DecayEngine.ModuleSDK.Exports.Attributes;

namespace DecayEngine.ModuleSDK.Exports.Capabilities
{
    [ScriptExportInterface("INameable", "Represents a nameable object.")]
    public interface INameableExport
    {
        [ScriptExportProperty("The name of the object.")]
        string Name { get; set; }
    }
}