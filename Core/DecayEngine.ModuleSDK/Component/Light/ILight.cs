using DecayEngine.DecPakLib.Math.Vector;

namespace DecayEngine.ModuleSDK.Component.Light
{
    public interface ILight : IComponent
    {
        Vector3 Color { get; set; }
        float Strength { get; set; }
    }
}