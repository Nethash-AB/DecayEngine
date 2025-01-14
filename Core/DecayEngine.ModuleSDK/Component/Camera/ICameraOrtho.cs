using DecayEngine.DecPakLib.Math.Vector;

namespace DecayEngine.ModuleSDK.Component.Camera
{
    public interface ICameraOrtho : ICamera
    {
        Vector2 ViewSpaceBBox { get; }
    }
}