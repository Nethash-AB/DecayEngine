using System.Collections.Generic;
using DecayEngine.DecPakLib.Resource.RootElement.AnimatedMaterial;
using DecayEngine.ModuleSDK.Object.Material;
using DecayEngine.ModuleSDK.Object.Texture.Texture2D;

namespace DecayEngine.ModuleSDK.Component.Material
{
    public interface IAnimatedMaterial : IMaterial, IComponent<AnimatedMaterialResource>
    {
        List<AnimationFrameElement> AnimationFrames { get; }

        IColorTexture DiffuseTexture { get; set; }
        INormalTexture NormalTexture { get; set; }
    }
}