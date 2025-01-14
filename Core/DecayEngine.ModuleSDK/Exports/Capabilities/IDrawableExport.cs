using DecayEngine.ModuleSDK.Exports.Attributes;

namespace DecayEngine.ModuleSDK.Exports.Capabilities
{
    [ScriptExportInterface("IDrawable", "Represents a drawable object.")]
    public interface IDrawableExport : IActivableExport, ITransformableExport
    {
        [ScriptExportProperty("Whether the `IDrawable` should be drawn to screen.\n" +
        "Setting this property to `false` does NOT unload the `IDrawable` from memory.\n" +
        "Using this property is significantly faster than (de)activating the `IDrawable` and should be preferred over (de)activation for animation.\n" +
        "Most `IDrawable` will default to `true` unless this property has explicitly been set before activating them.")]
        bool ShouldDraw { get; set; }
    }
}