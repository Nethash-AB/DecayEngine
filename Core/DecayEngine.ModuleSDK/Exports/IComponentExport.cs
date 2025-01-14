using DecayEngine.ModuleSDK.Exports.Attributes;
using DecayEngine.ModuleSDK.Exports.BaseExports.GameObject;
using DecayEngine.ModuleSDK.Exports.BaseExports.Scene;
using DecayEngine.ModuleSDK.Exports.Capabilities;

namespace DecayEngine.ModuleSDK.Exports
{
    [ScriptExportInterface("IComponent", "Represents an engine component.")]
    public interface IComponentExport : IManagedWrapper, IActivableExport, INameableExport, IParentableExport
    {
        [ScriptExportProperty("The Id of the resource the component was created from or null if the component type does not use resources.")]
        string Id { get; }
    }
}