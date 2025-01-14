using DecayEngine.DecPakLib.Math.Vector;
using DecayEngine.ModuleSDK.Object.Texture.RenderTargetTexture;

namespace DecayEngine.ModuleSDK.Object.Material
{
    public interface IRenderTargetMaterial : IMaterial
    {
        IRenderTargetColorTexture ColorTexture { get; }
        IRenderTargetDepthStencilTexture DepthStencilTexture { get; }

        void ReloadTextures(Vector2 size);
        void BindAsRenderTarget();
        void AttachColorComponents();
    }
}