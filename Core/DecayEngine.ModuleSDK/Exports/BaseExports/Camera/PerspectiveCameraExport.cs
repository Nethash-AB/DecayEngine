using DecayEngine.DecPakLib;
using DecayEngine.ModuleSDK.Component.Camera;
using DecayEngine.ModuleSDK.Exports.Attributes;
using DecayEngine.ModuleSDK.Math.Exports;

namespace DecayEngine.ModuleSDK.Exports.BaseExports.Camera
{
    [ScriptExportClass("PerspectiveCamera",
    "Represents a Perspective Camera Component.")]
    public class PerspectiveCameraExport : CameraExport
    {
        [ScriptExportProperty("The vertical viewing angle of the `PerspectiveCamera` (in radians).")]
        public float FieldOfView
        {
            get => ((ICameraPersp) Reference).FieldOfView;
            set => ((ICameraPersp) Reference).FieldOfView = value;
        }

        [ScriptExportConstructor]
        public PerspectiveCameraExport() : this(Vector3Export.Zero, QuaternionExport.Identity)
        {
        }

        [ScriptExportConstructor]
        public PerspectiveCameraExport(
            [ScriptExportParameter("The target position of the `PerspectiveCamera`.")] Vector3Export position
        ) : this(position, QuaternionExport.Identity)
        {
        }

        [ScriptExportConstructor]
        public PerspectiveCameraExport(
            [ScriptExportParameter("The target position of the `PerspectiveCamera`.")] Vector3Export position,
            [ScriptExportParameter("The target rotation of the `PerspectiveCamera`.")] QuaternionExport rotation
        ) : base(GameEngine.CreateComponent<ICameraPersp>(), position, rotation)
        {
        }

        [ExportCastConstructor]
        internal PerspectiveCameraExport(ByReference<ICamera> referencePointer) : base(referencePointer)
        {
        }

        [ExportCastConstructor]
        internal PerspectiveCameraExport(ICamera value) : base(value)
        {
        }
    }
}