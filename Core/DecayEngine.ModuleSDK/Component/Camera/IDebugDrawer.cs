using DecayEngine.DecPakLib.Math.Matrix;
using DecayEngine.DecPakLib.Math.Vector;
using DecayEngine.ModuleSDK.Capability;
using DecayEngine.ModuleSDK.Component.ShaderProgram;
using DecayEngine.ModuleSDK.Object.TextDrawer;

namespace DecayEngine.ModuleSDK.Component.Camera
{
    public interface IDebugDrawer : IActivable, IDestroyable
    {
        IShaderProgram DebugGeometryShaderProgram { get; set; }
        IShaderProgram DebugLinesShaderProgram { get; set; }
        IShaderProgram DebugTextShaderProgram { get; set; }
        ITextDrawer DebugTextDrawer { get; set; }

        void Draw(Matrix4 viewMatrix, Matrix4 projectionMatrix);

        void AddLine(Vector3 fromPosition, Vector3 toPosition, Vector4 color);
        void AddWireframeSquare(Vector3 position, Quaternion rotation, Vector3 scale, Vector4 color);
        void AddWireframeBox(Vector3 position, Quaternion rotation, Vector3 scale, Vector4 color);
        void AddText(Vector3 position, Quaternion rotation, Vector3 scale, Vector4 color, string text);
        void AddCrosshair(Vector3 position, Quaternion rotation, Vector3 scale);
        void AddPlanarGrid(Vector3 position, Quaternion rotation, Vector3 scale, int distance);
    }
}