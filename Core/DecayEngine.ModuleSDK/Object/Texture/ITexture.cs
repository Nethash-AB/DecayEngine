using DecayEngine.DecPakLib.Math.Vector;
using DecayEngine.ModuleSDK.Capability;

namespace DecayEngine.ModuleSDK.Object.Texture
{
    public interface ITexture : IActivable, INameable, IDestroyable
    {
        int Width { get; }
        int Height { get; }
        Vector2 Size { get; }

        void Bind();
        void Unbind();
    }
}