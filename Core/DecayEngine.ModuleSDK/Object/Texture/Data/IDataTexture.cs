using DecayEngine.DecPakLib.Math.Vector;

namespace DecayEngine.ModuleSDK.Object.Texture.Data
{
    public interface IDataTexture : ITexture
    {
        void Load(Vector2 size, int attachmentPoint);
        void Unload();
    }
}