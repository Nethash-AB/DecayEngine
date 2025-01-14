using DecayEngine.DecPakLib.Resource.RootElement.Font;
using DecayEngine.DecPakLib.Resource.RootElement.Shader;
using DecayEngine.DecPakLib.Resource.RootElement.ShaderProgram;
using DecayEngine.ModuleSDK.Engine;
using DecayEngine.ModuleSDK.Engine.Render;

// ReSharper disable FieldCanBeMadeReadOnly.Global
// ReSharper disable ConvertToConstant.Global

namespace DecayEngine.OpenGL
{
    public class OpenGlEngineOptions : IEngineOptions
    {
        public IGameSurface GameSurface;

        public bool IsEmbedded = false;
        public bool UseGeometryShaders = true;
        public bool UseCompressedTextures = true;
        public bool UseSpirvShaders = true;
        public ShaderLanguage FallbackShaderLanguage = ShaderLanguage.Glsl;

        public ShaderProgramResource DefaultPostProcessingShaderProgram = null;
        public ShaderProgramResource DefaultPbrLightingShaderProgram = null;

        public bool DrawDebug = false;
        public bool DrawDebugOriginCrosshair = false;
        public bool DrawDebugNames = false;
        public ShaderProgramResource DebugGeometryShaderProgram = null;
        public ShaderProgramResource DebugLinesShaderProgram = null;
        public ShaderProgramResource DebugTextShaderProgram = null;
        public FontResource DebugTextFont = null;
    }
}