using DecayEngine.DecPakLib.Math.Matrix;
using DecayEngine.DecPakLib.Math.Vector;
using DecayEngine.ModuleSDK.Object.Transform;

namespace DecayEngine.ModuleSDK.Capability
{
    public interface IDrawable : IActivable
    {
        Vector3 Pivot { get; }
        Vector3 DrawableSize { get; }
        bool ShouldDraw { get; set; }
        Transform WorldSpaceTransform { get; }
        bool IsPbrCapable { get; }

        void Draw(Matrix4 viewMatrix, Matrix4 projectionMatrix);
    }
}