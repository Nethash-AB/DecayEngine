using DecayEngine.DecPakLib;
using DecayEngine.DecPakLib.Math;
using DecayEngine.ModuleSDK.Exports;
using DecayEngine.ModuleSDK.Exports.Attributes;

namespace DecayEngine.ModuleSDK.Math.Exports
{
    [ScriptExportClass("Aabb", "Represents an Aabb object.", typeof(MathNamespaceExport))]
    public class AabbExport : ExportableManagedObject<Aabb>
    {
        public override int Type => (int) ManagedExportType.Aabb;
        public override string SubType => "Vector3";

        [ScriptExportProperty("The position of the minimum bounds of the `Aabb`.")]
        public Vector3Export Min
        {
            get => Reference.Min;
        }

        [ScriptExportProperty("The position of the maximum bounds of the `Aabb`.")]
        public Vector3Export Max
        {
            get => Reference.Max;
        }

        public AabbExport(Aabb aabb) : base(aabb)
        {
        }

        public AabbExport(ByReference<Aabb> referencePointer) : base(referencePointer)
        {
        }
    }
}