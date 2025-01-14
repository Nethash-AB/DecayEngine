using DecayEngine.ModuleSDK.Exports.Attributes;
using DecayEngine.ModuleSDK.Exports.BaseExports.Transform;

namespace DecayEngine.ModuleSDK.Exports.Capabilities
{
    [ScriptExportInterface("ITransformable", "Represents a transformable object.")]
    public interface ITransformableExport
    {
        [ScriptExportProperty("The transform of the object.")]
        TransformExport Transform { get; }

        [ScriptExportProperty("The transform of the object in world space.\n" +
        "This property takes into account the transforms of the parents of the object.\n" +
        "Changing the values returned by this property has no effect as the world space transform is calculated on demand.")]
        TransformExport WorldSpaceTransform { get; }
    }
}