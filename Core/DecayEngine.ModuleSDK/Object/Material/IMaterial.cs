using DecayEngine.DecPakLib.Math.Vector;
using DecayEngine.ModuleSDK.Capability;

namespace DecayEngine.ModuleSDK.Object.Material
{
    public interface IMaterial : IActivable, INameable, IDestroyable
    {
        Vector2 AspectRatio { get; }

        void Bind();
        void Unbind();
    }
}