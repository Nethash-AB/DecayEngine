using DecayEngine.DecPakLib.Math.Vector;

namespace DecayEngine.ModuleSDK.Object.Texture.RenderTargetTexture
{
    public interface IRenderTargetTexture : ITexture
    {
        void BindAsRender();
        void Load(Vector2 size, int attachmentPoint);
        void Unload();
    }
}