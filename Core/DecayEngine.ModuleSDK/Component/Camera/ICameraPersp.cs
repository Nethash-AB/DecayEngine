using DecayEngine.DecPakLib.Resource.RootElement.Texture3D;
using DecayEngine.ModuleSDK.Object.Texture.Texture3D;

namespace DecayEngine.ModuleSDK.Component.Camera
{
    public interface ICameraPersp : ICamera
    {
        float FieldOfView { get; set; }
        Texture3DResource EnvironmentTextureResource { get; set; }
        IEnvironmentTexture EnvironmentTexture { get; }
    }
}