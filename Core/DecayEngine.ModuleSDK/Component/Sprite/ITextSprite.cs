using DecayEngine.DecPakLib;
using DecayEngine.DecPakLib.Resource.RootElement.Font;
using DecayEngine.ModuleSDK.Capability;
using DecayEngine.ModuleSDK.Component.ShaderProgram;
using DecayEngine.ModuleSDK.Object.TextDrawer;

namespace DecayEngine.ModuleSDK.Component.Sprite
{
    public interface ITextSprite : ITextDrawer, IComponent<FontResource>, IDebugDrawable
    {
        ByReference<IShaderProgram> ShaderProgramByRef { get; }
    }
}