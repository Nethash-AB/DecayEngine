using DecayEngine.ModuleSDK.Exports.Attributes;

namespace DecayEngine.ModuleSDK.Exports.Capabilities
{
    [ScriptExportInterface("IActivable", "Represents an activable object.")]
    public interface IActivableExport
    {
        [ScriptExportProperty("The active status of the object.")]
        bool Active { get; set; }

        [ScriptExportProperty("The active status of the object in the inheritance graph.\n" +
        "`true` if the object and all its parents to the top of the inheritance graph are active, `false` otherwise.")]
        bool ActiveInGraph { get; }
    }
}