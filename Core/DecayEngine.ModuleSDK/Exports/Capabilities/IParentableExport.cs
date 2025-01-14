using DecayEngine.ModuleSDK.Exports.Attributes;
using DecayEngine.ModuleSDK.Exports.BaseExports.GameObject;
using DecayEngine.ModuleSDK.Exports.BaseExports.Scene;

namespace DecayEngine.ModuleSDK.Exports.Capabilities
{
    [ScriptExportInterface("IParentable", "Represents an object that can be parented to another object.")]
    public interface IParentableExport
    {
        [ScriptExportProperty("The parent of the object.", new [] {typeof(GameObjectExport), typeof(SceneExport)})]
        object Parent { get; }
    }
}