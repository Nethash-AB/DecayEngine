using DecayEngine.DecPakLib.Math.Vector;
using DecayEngine.DecPakLib.Resource.RootElement.Font;
using DecayEngine.ModuleSDK.Capability;
using DecayEngine.ModuleSDK.Component.ShaderProgram;
using DecayEngine.ModuleSDK.Component.Sprite;

namespace DecayEngine.ModuleSDK.Object.TextDrawer
{
    public interface ITextDrawer : IDestroyable, IDrawable, ITransformable, IResourceable<FontResource>
    {
        bool AutoUpdateOnChange { get; set; }
        Vector4 Color { get; set; }
        string Text { get; set; }
        float FontSize { get; set; }
        float CharacterSeparation { get; set; }
        float WhiteSpaceSeparation { get; set; }
        TextAlignmentHorizontal AlignmentHorizontal { get; set; }
        TextAlignmentVertical AlignmentVertical { get; set; }

        IShaderProgram ShaderProgram { get; set; }

        void Update();
    }
}