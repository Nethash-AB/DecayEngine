using DecayEngine.DecPakLib;
using DecayEngine.ModuleSDK.Object.Material;
using DecayEngine.ModuleSDK.Object.Sprite;

namespace DecayEngine.ModuleSDK.Component.Sprite
{
    public interface IAnimatedSprite : ISprite, IComponent
    {
        int Frame { get; set; }

        IMaterial Material { get; set; }
        ByReference<IMaterial> MaterialByRef { get; }
    }
}