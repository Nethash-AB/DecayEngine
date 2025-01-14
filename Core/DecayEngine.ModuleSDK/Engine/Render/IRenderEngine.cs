using System.Collections.Generic;
using DecayEngine.DecPakLib.Resource.RootElement.PostProcessing;
using DecayEngine.DecPakLib.Resource.RootElement.Shader;
using DecayEngine.DecPakLib.Resource.RootElement.ShaderProgram;
using DecayEngine.ModuleSDK.Component.Camera;
using DecayEngine.ModuleSDK.Component.ShaderProgram;
using DecayEngine.ModuleSDK.Object.FrameBuffer;

namespace DecayEngine.ModuleSDK.Engine.Render
{
    public interface IRenderEngine : IMultiThreadedEngine
    {
        bool IsEmbedded { get; }
        ShaderLanguage FallbackShaderLanguage { get; }

        ShaderProgramResource DefaultPostProcessingShaderProgram { get; }
        ShaderProgramResource DefaultPbrLightingShaderProgram { get; }

        IEnumerable<ICamera> Cameras { get; }
        IFrameBuffer ActiveFrameBufferRead { get; set; }
        IFrameBuffer ActiveFrameBufferWrite { get; set; }
        IShaderProgram ActiveShaderProgram { get; set; }

        IEnumerable<IRenderFrameBuffer> GlobalFrameBuffers { get; }

        bool DepthTestEnabled { get; set; }
        bool WireFrameEnabled { get; set; }

        bool DrawDebug { get; set; }
        bool DrawDebugOriginCrosshair { get; set; }
        bool DrawDebugNames { get; set; }

        bool SupportsFeature(RenderEngineFeatures feature);

        void TrackCamera(ICamera camera);
        void UntrackCamera(ICamera camera);

        IDeferredShadingFrameBuffer CreateDeferredShadingFrameBuffer();
        IRenderFrameBuffer CreateRenderFrameBuffer(PostProcessingStage postProcessingStage = null);
    }

    public interface IRenderEngine<in TOptions> : IRenderEngine, IMultiThreadedEngine<TOptions> where TOptions : IEngineOptions
    {
    }
}