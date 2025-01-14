using DecayEngine.DecPakLib;
using DecayEngine.ModuleSDK.Capability;
using DecayEngine.ModuleSDK.Component.ShaderProgram;

namespace DecayEngine.ModuleSDK.Object.Sprite
{
    public interface ISprite : INameable, IDestroyable, IDebugDrawable, ITransformable
    {
        IShaderProgram ShaderProgram { get; set; }
        ByReference<IShaderProgram> ShaderProgramByRef { get; }

        bool MaintainAspectRatio { get; set; }
    }
}