using DecayEngine.DecPakLib;
using DecayEngine.ModuleSDK.Capability;
using DecayEngine.ModuleSDK.Component.Material;
using DecayEngine.ModuleSDK.Exports.Attributes;
using DecayEngine.ModuleSDK.Exports.BaseExports.GameObject;

namespace DecayEngine.ModuleSDK.Exports.BaseExports.AnimatedMaterial
{
    [ScriptExportClass("AnimatedMaterial", "Represents an Animated Material Component.")]
    public class AnimatedMaterialExport : ExportableManagedObject<IAnimatedMaterial>, IComponentExport
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

        public string Id => Reference.Resource.Id;

        public object Parent => Reference.Parent != null ? new GameObjectExport(Reference.Parent) : null;

        public override int Type => (int) ManagedExportType.Material;

        [ExportCastConstructor]
        internal AnimatedMaterialExport(ByReference<IAnimatedMaterial> referencePointer) : base(referencePointer)
        {
        }

        [ExportCastConstructor]
        internal AnimatedMaterialExport(IAnimatedMaterial value) : base(value)
        {
        }
    }
}