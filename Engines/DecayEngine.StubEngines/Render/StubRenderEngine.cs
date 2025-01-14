using System.Collections.Generic;
using System.Threading.Tasks;
using DecayEngine.DecPakLib.Resource.RootElement.PostProcessing;
using DecayEngine.DecPakLib.Resource.RootElement.Shader;
using DecayEngine.DecPakLib.Resource.RootElement.ShaderProgram;
using DecayEngine.ModuleSDK.Component;
using DecayEngine.ModuleSDK.Component.Camera;
using DecayEngine.ModuleSDK.Component.ShaderProgram;
using DecayEngine.ModuleSDK.Engine.Render;
using DecayEngine.ModuleSDK.Engine.Script;
using DecayEngine.ModuleSDK.Object.FrameBuffer;
using DecayEngine.ModuleSDK.Threading;

namespace DecayEngine.StubEngines.Render
{
    public class StubRenderEngine : IRenderEngine<StubEngineOptions>
    {
        private readonly List<ICamera> _cameras;

        public IEngineThread EngineThread { get; private set; }

        public List<IComponentFactory> ComponentFactories { get; }
        public ScriptExports ScriptExports { get; }

        public bool IsEmbedded => false;
        public ShaderLanguage FallbackShaderLanguage => ShaderLanguage.Glsl;

        public ShaderProgramResource DefaultPostProcessingShaderProgram => null;
        public ShaderProgramResource DefaultPbrLightingShaderProgram => null;

        public IEnumerable<ICamera> Cameras => _cameras;

        public IFrameBuffer ActiveFrameBufferRead
        {
            get => null;
            set {}
        }

        public IFrameBuffer ActiveFrameBufferWrite
        {
            get => null;
            set {}
        }

        public IShaderProgram ActiveShaderProgram
        {
            get => null;
            set {}
        }

        public IEnumerable<IRenderFrameBuffer> GlobalFrameBuffers => new List<IRenderFrameBuffer>();

        public bool DepthTestEnabled { get; set; }
        public bool WireFrameEnabled { get; set; }

        public bool DrawDebug
        {
            get => true;
            set {}
        }
        public bool DrawDebugOriginCrosshair
        {
            get => true;
            set {}
        }
        public bool DrawDebugNames
        {
            get => true;
            set {}
        }

        public StubRenderEngine()
        {
            _cameras = new List<ICamera>();
            ComponentFactories = new List<IComponentFactory>();
            ScriptExports = new ScriptExports();
        }

        public Task Init(StubEngineOptions options)
        {
            EngineThread = new ManagedEngineThread("", 128);
            EngineThread.Run();

            return Task.CompletedTask;
        }

        public Task Shutdown()
        {
            EngineThread.Stop();
            return Task.CompletedTask;
        }

        public bool SupportsFeature(RenderEngineFeatures feature)
        {
            return false;
        }

        public void TrackCamera(ICamera camera)
        {
            if (_cameras.Contains(camera)) return;

            _cameras.Add(camera);
        }

        public void UntrackCamera(ICamera camera)
        {
            if (!_cameras.Contains(camera)) return;

            _cameras.Remove(camera);
        }

        public IDeferredShadingFrameBuffer CreateDeferredShadingFrameBuffer()
        {
            return null;
        }

        public IRenderFrameBuffer CreateRenderFrameBuffer(PostProcessingStage postProcessingStage = null)
        {
            return null;
        }
    }
}