using DecayEngine.DecPakLib.Math.Vector;
using DecayEngine.DecPakLib.Resource.RootElement.PbrMaterial;
using DecayEngine.ModuleSDK.Object.Material;
using DecayEngine.ModuleSDK.Object.Texture.Texture2D;

namespace DecayEngine.ModuleSDK.Component.Material
{
    public interface IPbrMaterial : IMaterial, IComponent<PbrMaterialResource>
    {
        IColorTexture AlbedoTexture { get; set; }
        Vector4 AlbedoColor { get; set; }
        INormalTexture NormalTexture { get; set; }
        IMetallicityTexture MetallicityTexture { get; set; }
        float MetallicityFactor { get; set; }
        IRoughnessTexture RoughnessTexture { get; set; }
        float RoughnessFactor { get; set; }
        IEmissionTexture EmissionTexture { get; set; }
        Vector4 EmissionColor { get; set; }
    }
}