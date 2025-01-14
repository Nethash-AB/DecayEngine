using DecayEngine.DecPakLib;
using DecayEngine.ModuleSDK.Exports.Attributes;
using DecayEngine.ModuleSDK.Math.Exports;

namespace DecayEngine.ModuleSDK.Exports.BaseExports.Transform
{
    [ScriptExportClass("Transform", "Represents the position, rotation and scale in space of a Component.")]
    public class TransformExport : ExportableManagedObject<Object.Transform.Transform>
    {
        [ScriptExportProperty("The position of the `Transform`.")]
        public Vector3Export Position
        {
            get => new Vector3Export(Reference.Position);
            set => Reference.Position = value;
        }

        [ScriptExportProperty("The rotation of the `Transform` (in degrees).")]
        public QuaternionExport Rotation
        {
            get => new QuaternionExport(Reference.Rotation);
            set => Reference.Rotation = value;
        }

        [ScriptExportProperty("The scale of the `Transform`.")]
        public Vector3Export Scale
        {
            get => new Vector3Export(Reference.Scale);
            set => Reference.Scale = value;
        }

        [ScriptExportProperty("The right vector of the `Transform`.")]
        public Vector3Export Right => Reference.Right;

        [ScriptExportProperty("The up vector of the `Transform`.")]
        public Vector3Export Up => Reference.Up;

        [ScriptExportProperty("The forward vector of the `Transform`.")]
        public Vector3Export Forward => Reference.Right;

        public override int Type => (int) ManagedExportType.Transform;

        [ExportCastConstructor]
        public TransformExport(ByReference<Object.Transform.Transform> referencePointer) : base(referencePointer)
        {
        }

        [ExportCastConstructor]
        public TransformExport(Object.Transform.Transform value) : base(value)
        {
        }
    }
}