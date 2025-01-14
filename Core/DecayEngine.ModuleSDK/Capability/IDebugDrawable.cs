using DecayEngine.DecPakLib.Math.Matrix;
using DecayEngine.ModuleSDK.Component.Camera;
using DecayEngine.ModuleSDK.Math;

namespace DecayEngine.ModuleSDK.Capability
{
    public interface IDebugDrawable : IDrawable
    {
        void DrawDebug(Matrix4 viewMatrix, Matrix4 projectionMatrix, IDebugDrawer debugDrawer);
    }
}