using DecayEngine.DecPakLib;
using DecayEngine.ModuleSDK.Capability;
using DecayEngine.ModuleSDK.Exports.Attributes;
using DecayEngine.ModuleSDK.Exports.BaseExports.GameObject;
using DecayEngine.ModuleSDK.Exports.BaseExports.Scene;
using DecayEngine.ModuleSDK.Object;
using DecayEngine.ModuleSDK.Object.Texture;

namespace DecayEngine.ModuleSDK.Exports.BaseExports.Texture
{
    [ScriptExportClass("Texture", "Represents a Texture.")]
    public class TextureExport : ExportableManagedObject<ITexture>
    {
        public bool Active
        {
            get => Reference.Active;
            set => Reference.Active = value;
        }

        public bool ActiveInGraph => Reference.ActiveInGraph();

        public string Name
        {
            get => Reference.Name;
            set => Reference.Name = value;
        }

        public override int Type => (int) ManagedExportType.Texture;

        [ExportCastConstructor]
        internal TextureExport(ByReference<ITexture> referencePointer) : base(referencePointer)
        {
        }

        [ExportCastConstructor]
        internal TextureExport(ITexture value) : base(value)
        {
        }
    }
}