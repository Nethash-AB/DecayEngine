using DecayEngine.ModuleSDK.Exports.Attributes;

namespace DecayEngine.ModuleSDK.Exports
{
    [ScriptExportInterface("IManagedWrapper", "Represents a managed wrapper.")]
    public interface IManagedWrapper
    {
        [ScriptExportProperty("The Managed Type of the object.", typeof(ManagedExportType))]
        int Type { get; }

        string SubType { get; }
    }
}