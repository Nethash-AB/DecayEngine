using DecayEngine.DecPakLib.Resource.RootElement.Font;
using DecayEngine.DecPakLib.Resource.RootElement.ShaderProgram;
using DecayEngine.ModuleSDK;
using DecayEngine.ModuleSDK.Component.ShaderProgram;
using DecayEngine.ModuleSDK.Component.Sprite;
using DecayEngine.ModuleSDK.Engine.Render;
using DecayEngine.ModuleSDK.Object;
using DecayEngine.ModuleSDK.Object.TextDrawer;
using DecayEngine.OpenGL.Object.Text;

// ReSharper disable SuggestBaseTypeForParameter
namespace DecayEngine.OpenGL
{
    public readonly struct DefaultDebugComponents
    {
        public readonly IShaderProgram DebugGeometryShaderProgram;
        public readonly IShaderProgram DebugLinesShaderProgram;
        public readonly IShaderProgram DebugTextShaderProgram;
        public readonly ITextDrawer DebugTextDrawer;

        public DefaultDebugComponents(
            ShaderProgramResource debugGeometryShaderProgramResource,
            ShaderProgramResource debugLinesShaderProgramResource,
            ShaderProgramResource debugTextShaderProgramResource,
            FontResource debugTextFont
        ) : this()
        {

            if (debugGeometryShaderProgramResource != null)
            {
                DebugGeometryShaderProgram = (IShaderProgram) GameEngine.CreateComponent(debugGeometryShaderProgramResource);
                GameEngine.ActiveScene.AttachComponent(DebugGeometryShaderProgram);
                DebugGeometryShaderProgram.Persistent = true;
                DebugGeometryShaderProgram.Active = true;
            }

            if (debugLinesShaderProgramResource != null)
            {
                DebugLinesShaderProgram = (IShaderProgram) GameEngine.CreateComponent(debugLinesShaderProgramResource);
                GameEngine.ActiveScene.AttachComponent(DebugLinesShaderProgram);
                DebugLinesShaderProgram.Persistent = true;
                DebugLinesShaderProgram.Active = true;
            }

            if (debugTextShaderProgramResource != null)
            {
                DebugTextShaderProgram = (IShaderProgram) GameEngine.CreateComponent(debugTextShaderProgramResource);
                GameEngine.ActiveScene.AttachComponent(DebugTextShaderProgram);
                DebugTextShaderProgram.Persistent = true;
                DebugTextShaderProgram.Active = true;
            }

            if (debugTextFont != null)
            {
                DebugTextDrawer = new TextDrawer
                {
                    Resource = debugTextFont,
                    ShaderProgram = DebugTextShaderProgram,
                    FontSize = 15f,
                    AlignmentHorizontal = TextAlignmentHorizontal.Center,
                    AlignmentVertical = TextAlignmentVertical.Center,
                    Active = true
                };

            }
        }
    }
}