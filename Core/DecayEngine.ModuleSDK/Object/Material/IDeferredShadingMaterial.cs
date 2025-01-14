using DecayEngine.ModuleSDK.Object.Texture.Data;
using DecayEngine.ModuleSDK.Object.Texture.Texture3D;

namespace DecayEngine.ModuleSDK.Object.Material
{
    public interface IDeferredShadingMaterial : IRenderTargetMaterial
    {
        IDataTexture PositionTexture { get; }
        IDataTexture NormalsTexture { get; }
        IDataTexture MetallicityRoughnessTexture { get; }
        IDataTexture EmissionTexture { get; }
        IEnvironmentTexture EnvironmentTexture { get; set; }
    }
}