using DecayEngine.DecPakLib;
using DecayEngine.ModuleSDK.Component.Camera;
using DecayEngine.ModuleSDK.Exports.Attributes;
using DecayEngine.ModuleSDK.Math.Exports;

namespace DecayEngine.ModuleSDK.Exports.BaseExports.Camera
{
    [ScriptExportClass("OrthographicCamera",
        "Represents an Orthographic Camera Component.")]
    public class OrthographicCameraExport : CameraExport
    {
        [ScriptExportConstructor]
        public OrthographicCameraExport() : this(Vector3Export.Zero, QuaternionExport.Identity)
        {
        }

        [ScriptExportConstructor]
        public OrthographicCameraExport(
            [ScriptExportParameter("The target position of the `OrthographicCamera`.")] Vector3Export position
        ) : this(position, QuaternionExport.Identity)
        {
        }

        [ScriptExportConstructor]
        public OrthographicCameraExport(
            [ScriptExportParameter("The target position of the `OrthographicCamera`.")] Vector3Export position,
            [ScriptExportParameter("The target rotation of the `OrthographicCamera`.")] QuaternionExport rotation
        ) : base(GameEngine.CreateComponent<ICameraOrtho>(), position, rotation)
        {
        }

        [ExportCastConstructor]
        internal OrthographicCameraExport(ByReference<ICamera> referencePointer) : base(referencePointer)
        {
        }

        [ExportCastConstructor]
        internal OrthographicCameraExport(ICamera value) : base(value)
        {
        }
    }
}