using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using DecayEngine.DecPakLib.Math.Matrix;
using DecayEngine.DecPakLib.Math.Vector;
using DecayEngine.DecPakLib.Resource.RootElement.PostProcessing;
using DecayEngine.DecPakLib.Resource.RootElement.Shader;
using DecayEngine.DecPakLib.Resource.RootElement.ShaderProgram;
using DecayEngine.ModuleSDK;
using DecayEngine.ModuleSDK.Capability;
using DecayEngine.ModuleSDK.Component;
using DecayEngine.ModuleSDK.Component.Camera;
using DecayEngine.ModuleSDK.Component.ShaderProgram;
using DecayEngine.ModuleSDK.Component.Sprite;
using DecayEngine.ModuleSDK.Engine.Render;
using DecayEngine.ModuleSDK.Engine.Script;
using DecayEngine.ModuleSDK.Logging;
using DecayEngine.ModuleSDK.Object.FrameBuffer;
using DecayEngine.ModuleSDK.Object.GameObject;
using DecayEngine.ModuleSDK.Object.Scene;
using DecayEngine.ModuleSDK.Threading;
using DecayEngine.OpenGL.Component.AnimatedMaterial;
using DecayEngine.OpenGL.Component.AnimatedSprite;
using DecayEngine.OpenGL.Component.Camera;
using DecayEngine.OpenGL.Component.Light;
using DecayEngine.OpenGL.Component.Mesh;
using DecayEngine.OpenGL.Component.PbrMaterial;
using DecayEngine.OpenGL.Component.RenderTargetSprite;
using DecayEngine.OpenGL.Component.Shader;
using DecayEngine.OpenGL.Component.ShaderProgram;
using DecayEngine.OpenGL.Component.TextSprite;
using DecayEngine.OpenGL.Debug;
using DecayEngine.OpenGL.Object.FrameBuffer;
using DecayEngine.OpenGL.OpenGLInterop;

namespace DecayEngine.OpenGL
{
    public class OpenGlEngine : IRenderEngine<OpenGlEngineOptions>
    {
        private IGameSurface _gameSurface;
        private readonly Delegates.DebugProc _onGlDebugMessage = OnGlDebugMessage;

        private RenderEngineFeatures _supportedEngineFeatures;

        private DefaultDebugComponents _defaultDebugComponents;

        private readonly List<ICamera> _cameras;
        private IRenderFrameBuffer _geometryPerspFrameBuffer;
        private IRenderFrameBuffer _geometryOrthoFrameBuffer;

        private bool _depthTestEnabled;
        private bool _wireFrameEnabled;

        public IEngineThread EngineThread { get; private set; }

        public List<IComponentFactory> ComponentFactories { get; }
        public ScriptExports ScriptExports { get; }

        public bool IsEmbedded { get; private set; }
        public ShaderLanguage FallbackShaderLanguage { get; private set; }

        public ShaderProgramResource DefaultPostProcessingShaderProgram { get; private set; }
        public ShaderProgramResource DefaultPbrLightingShaderProgram { get; private set; }

        public IEnumerable<ICamera> Cameras => _cameras;
        public IFrameBuffer ActiveFrameBufferRead { get; set; }
        public IFrameBuffer ActiveFrameBufferWrite { get; set; }
        public IShaderProgram ActiveShaderProgram { get; set; }

        public IEnumerable<IRenderFrameBuffer> GlobalFrameBuffers => new List<IRenderFrameBuffer>
        {
            _geometryPerspFrameBuffer,
            _geometryOrthoFrameBuffer
        };

        public bool DepthTestEnabled
        {
            get => _depthTestEnabled;
            set
            {
                if (!_depthTestEnabled && value)
                {
                    GL.Enable(EnableCap.DepthTest);
                    _depthTestEnabled = true;
                }
                else if (_depthTestEnabled && !value)
                {
                    GL.Disable(EnableCap.DepthTest);
                    _depthTestEnabled = false;
                }
            }
        }

        public bool WireFrameEnabled
        {
            get => _wireFrameEnabled;
            set
            {
                if (!_wireFrameEnabled && value)
                {
                    GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
                    GL.Enable(EnableCap.PolygonOffsetLine);
                    GL.PolygonOffset(-1f, -1f);

                    _wireFrameEnabled = true;
                }
                else if (_wireFrameEnabled && !value)
                {
                    GL.Disable(EnableCap.PolygonOffsetLine);
                    GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
                    GL.PolygonOffset(0f, 0f);

                    _wireFrameEnabled = false;
                }
            }
        }

        public bool DrawDebug { get; set; }

        public bool DrawDebugOriginCrosshair { get; set; }
        public bool DrawDebugNames { get; set; }

        public OpenGlEngine()
        {
            ComponentFactories = new List<IComponentFactory>
            {
                new AnimatedMaterialFactory(),
                new ShaderFactory(),
                new ShaderProgramFactory(),
                new PerspectiveCameraFactory(),
                new OrthographicCameraFactory(),
                new AnimatedSpriteFactory(),
                new RenderTargetSpriteFactory(),
                new TextSpriteFactory(),
                new MeshFactory(),
                new PbrMaterialFactory(),
                new LightFactory()
            };

            ScriptExports = new ScriptExports();
            _cameras = new List<ICamera>();
        }

        public Task Init(OpenGlEngineOptions options)
        {
            _gameSurface =
                options.GameSurface ?? throw new ArgumentNullException(nameof(options.GameSurface), "OpenGL requires a surface to render to.");

            IsEmbedded = options.IsEmbedded;

            if (options.UseGeometryShaders)
            {
                _supportedEngineFeatures |= RenderEngineFeatures.GeometryShaders;
            }
            if (options.UseCompressedTextures)
            {
                _supportedEngineFeatures |= RenderEngineFeatures.CompressedTextures;
            }
            if (options.UseSpirvShaders)
            {
                _supportedEngineFeatures |= RenderEngineFeatures.SpirvShaders;
            }

            FallbackShaderLanguage = options.FallbackShaderLanguage;

            DefaultPostProcessingShaderProgram = options.DefaultPostProcessingShaderProgram;
            DefaultPbrLightingShaderProgram = options.DefaultPbrLightingShaderProgram;

            if (options.DrawDebug)
            {
                DrawDebugOriginCrosshair = options.DrawDebugOriginCrosshair;
                DrawDebugNames = options.DrawDebugNames;

                DrawDebug = true;

                GameEngine.OnScenePreload += (scene, isInit) =>
                {
                    if (!isInit) return;

                    _defaultDebugComponents = new DefaultDebugComponents(
                        options.DebugGeometryShaderProgram,
                        options.DebugLinesShaderProgram,
                        options.DebugTextShaderProgram,
                        options.DebugTextFont
                    );
                };
            }

            GameEngine.OnScenePreload += (scene, isInit) =>
            {
                if (!isInit) return;

                IShaderProgram fboShaderProgram = (IShaderProgram) GameEngine.CreateComponent(DefaultPostProcessingShaderProgram);
                scene.AttachComponent(fboShaderProgram);
                fboShaderProgram.Persistent = true;
                fboShaderProgram.Active = true;

                _geometryPerspFrameBuffer = CreateRenderFrameBuffer();
                _geometryPerspFrameBuffer.Name = "geometryPersp";
                _geometryPerspFrameBuffer.ShaderProgram = fboShaderProgram;
                _geometryPerspFrameBuffer.Size = _gameSurface.Size;
                _geometryPerspFrameBuffer.Active = true;

                _geometryOrthoFrameBuffer = CreateRenderFrameBuffer();
                _geometryOrthoFrameBuffer.Name = "geometryOrtho";
                _geometryOrthoFrameBuffer.ShaderProgram = fboShaderProgram;
                _geometryOrthoFrameBuffer.Size = _gameSurface.Size;
                _geometryOrthoFrameBuffer.Active = true;
            };

            GL.GetAddress = _gameSurface.GetRenderFunctionPtr;
            _gameSurface.OnInit += InitGl;
            _gameSurface.OnResize += OnResize;
            _gameSurface.OnQuit += () => GameEngine.Shutdown();

            EngineThread = _gameSurface.UiThread ?? new ManagedEngineThread("OpenGL", 240);

            Task initTask = EngineThread.ExecuteOnThreadAsync(InitEngine);

            EngineThread.Run();
            return initTask;
        }

        public Task Shutdown()
        {
            EngineThread.Stop();

            _gameSurface.Quit();
            _gameSurface = null;

            GameEngine.LogAppendLine(LogSeverity.Info, "OpenGL", "OpenGL terminated.");
            return Task.CompletedTask;
        }

        public bool SupportsFeature(RenderEngineFeatures feature)
        {
            return _supportedEngineFeatures.HasFlag(feature);
        }

        public void TrackCamera(ICamera camera)
        {
            if (_cameras.Contains(camera)) return;

            camera.DebugDrawer = new OpenGlDebugDrawer(in _defaultDebugComponents);

            _cameras.Add(camera);
            camera.Transform.Scale = new Vector3(_gameSurface.Size.X, _gameSurface.Size.Y, 1f);
        }

        public void UntrackCamera(ICamera camera)
        {
            if (!_cameras.Contains(camera)) return;

            _cameras.Remove(camera);
        }

        public IDeferredShadingFrameBuffer CreateDeferredShadingFrameBuffer()
        {
            return new DeferredShadingFrameBuffer();
        }

        public IRenderFrameBuffer CreateRenderFrameBuffer(PostProcessingStage postProcessingStage = null)
        {
            return new RenderFrameBuffer
            {
                PostProcessingStage = postProcessingStage
            };
        }

        private void InitEngine()
        {
            _gameSurface.Run().Wait();
            EngineThread.OnUpdate += Loop;

            GameEngine.LogAppendLine(LogSeverity.Info, "OpenGL", $"OpenGL loaded. Thread ID: {EngineThread.ThreadId}");
        }

        private void Loop(double deltaTime)
        {
#if DEBUG
            _gameSurface.Title =
                // ToDo: Move "Decay Engine" somewhere else where it isn't hardcoded.
                $"Decay Engine ({GL.GetString(StringName.Version)}) FPS: {1f / deltaTime : 0} | " +
                $"VSync: {_gameSurface.VSync}, FullScreen: {_gameSurface.FullScreen}, " +
                $"Borderless: {_gameSurface.Borderless}, Maximized: {_gameSurface.Maximized} | " +
                $"DebugDraw: {GameEngine.RenderEngine.DrawDebug}, DrawOriginCrosshair: {GameEngine.RenderEngine.DrawDebugOriginCrosshair}, " +
                $"DrawNames: {GameEngine.RenderEngine.DrawDebugNames}, DrawColliders: {GameEngine.PhysicsEngine?.PhysicsWorld?.DrawDebug ?? false}";
#endif

            RunRenderables((float) deltaTime);
            RenderScene();

            _gameSurface.Update();
        }

        private void InitGl()
        {
            GL.ReloadFunctions();

            GL.Enable(EnableCap.DebugOutput);
            GL.DebugMessageCallback(_onGlDebugMessage, IntPtr.Zero);

            if (!IsEmbedded)
            {
                GL.Enable(EnableCap.Multisample);
            }

            GL.Enable(EnableCap.CullFace);
            GL.FrontFace(FrontFaceDirection.Ccw);
            GL.CullFace(CullFaceMode.Back);

            DepthTestEnabled = true;
            GL.DepthFunc(DepthFunction.Less);

            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);

            GameEngine.LogAppendLine(LogSeverity.Info, "OpenGL", $"OpenGL Version: {GL.GetString(StringName.Version)}");
            GameEngine.LogAppendLine(LogSeverity.Info, "OpenGL", $"OpenGL Vendor: {GL.GetString(StringName.Vendor)}");
            GameEngine.LogAppendLine(LogSeverity.Info, "OpenGL", $"OpenGL Renderer: {GL.GetString(StringName.Renderer)}");
            GameEngine.LogAppendLine(LogSeverity.Info, "OpenGL", $"OpenGL ShadingLanguageVersion: {GL.GetString(StringName.ShadingLanguageVersion)}");

            int numExtensions = GL.GetInteger(GetPName.NumExtensions);
            string[] extensions = new string[numExtensions];
            for (uint i = 0; i < numExtensions; i++)
            {
                string extension = GL.GetStringi(StringName.Extensions, i);
                extensions[i] = extension;

//                switch (extension.ToLowerInvariant())
//                {
//                    case "gl_ext_srgb" when !SupportsFeature(RenderEngineFeatures.Srgb):
//                        _supportedEngineFeatures |= RenderEngineFeatures.Srgb;
//                        break;
//                    case "gl_ext_texture_srgb" when !SupportsFeature(RenderEngineFeatures.Srgb):
//                        _supportedEngineFeatures |= RenderEngineFeatures.Srgb;
//                        break;
//                    case "gl_ext_framebuffer_srgb" when !SupportsFeature(RenderEngineFeatures.Srgb):
//                        _supportedEngineFeatures |= RenderEngineFeatures.Srgb;
//                        break;
//                    case "gl_arb_framebuffer_srgb" when !SupportsFeature(RenderEngineFeatures.Srgb):
//                        _supportedEngineFeatures |= RenderEngineFeatures.Srgb;
//                        break;
//                }
            }
            GameEngine.LogAppendLine(LogSeverity.Debug, "OpenGL", $"OpenGL SupportedExtensions: {string.Join(", ", extensions)}");

//            if (SupportsFeature(RenderEngineFeatures.Srgb))
//            {
//                GL.Enable(EnableCap.FramebufferSrgb);
//            }

//            GL.ClearColor(0f, 0f, 0f, 0f);

            UpdateViewport(_gameSurface.Size);
        }

        private void UpdateViewport(Vector2 size)
        {
            GL.Viewport(0, 0, (int) size.X, (int) size.Y);

            if (_geometryPerspFrameBuffer != null)
            {
                _geometryPerspFrameBuffer.Size = size;
            }

            if (_geometryOrthoFrameBuffer != null)
            {
                _geometryOrthoFrameBuffer.Size = size;
            }

            foreach (ICamera camera in Cameras)
            {
                if (!camera.ManualSize)
                {
                    camera.Transform.Scale = new Vector3(size.X, size.Y, 1f);
                }

                foreach (ITextSprite textSprite in camera.Drawables.OfType<ITextSprite>())
                {
                    textSprite.Update();
                }
            }
        }

        // ToDo: Implement this properly once the input system is in place: https://wiki.libsdl.org/SDLKeycodeLookup
//        private void OnEvent(SDL.SDL_Event sdlEvent)
//        {
//            if (sdlEvent.type == SDL.SDL_EventType.SDL_KEYUP)
//            {
//#if DEBUG
//                if (sdlEvent.key.keysym.sym == SDL.SDL_Keycode.SDLK_ESCAPE)
//                {
//                    _gameWindow.Quit();
//                }
//                else if (sdlEvent.key.keysym.sym == SDL.SDL_Keycode.SDLK_F1)
//                {
//                    _gameWindow.VSync = !_gameWindow.VSync;
//                }
//                else if (sdlEvent.key.keysym.sym == SDL.SDL_Keycode.SDLK_F2)
//                {
//                    _gameWindow.FullScreen = !_gameWindow.FullScreen;
//                }
//                else if (sdlEvent.key.keysym.sym == SDL.SDL_Keycode.SDLK_F3)
//                {
//                    _gameWindow.Borderless = !_gameWindow.Borderless;
//                }
//                else if (sdlEvent.key.keysym.sym == SDL.SDL_Keycode.SDLK_F4)
//                {
//                    _gameWindow.Maximized = !_gameWindow.Maximized;
//                }
//                else if (sdlEvent.key.keysym.sym == SDL.SDL_Keycode.SDLK_F5)
//                {
//                    DrawDebug = !DrawDebug;
//                }
//                else if (sdlEvent.key.keysym.sym == SDL.SDL_Keycode.SDLK_F6)
//                {
//                    DrawDebugOriginCrosshair = !DrawDebugOriginCrosshair;
//                }
//                else if (sdlEvent.key.keysym.sym == SDL.SDL_Keycode.SDLK_F7)
//                {
//                    DrawDebugNames = !DrawDebugNames;
//                }
//                else if (sdlEvent.key.keysym.sym == SDL.SDL_Keycode.SDLK_F8)
//                {
//                    if (GameEngine.PhysicsEngine != null)
//                    {
//                        GameEngine.PhysicsEngine.DrawDebug = !GameEngine.PhysicsEngine.DrawDebug;
//                    }
//                }
//                else if (sdlEvent.key.keysym.sym == SDL.SDL_Keycode.SDLK_UP)
//                {
//                    ICameraPersp cam = GameEngine.ActiveScene.Components.OfType<ICameraPersp>().FirstOrDefault();
//                    if (cam != null)
//                    {
//                        cam.Transform.Position += new Vector3(0, 0, 0.25f);
//                    }
//                }
//                else if (sdlEvent.key.keysym.sym == SDL.SDL_Keycode.SDLK_DOWN)
//                {
//                    ICameraPersp cam = GameEngine.ActiveScene.Components.OfType<ICameraPersp>().FirstOrDefault();
//                    if (cam != null)
//                    {
//                        cam.Transform.Position -= new Vector3(0, 0, 0.25f);
//                    }
//                }
//                else if (sdlEvent.key.keysym.sym == SDL.SDL_Keycode.SDLK_KP_8)
//                {
//                    ICameraOrtho cam = GameEngine.ActiveScene.Components.OfType<ICameraOrtho>().FirstOrDefault();
//                    if (cam != null)
//                    {
//                        foreach (IDrawable camDrawable in cam.Drawables)
//                        {
//                            camDrawable.Transform.Position *= new Vector3(2f, 2f, 1f);
//                        }
//                        cam.Transform.Scale *= new Vector3(2f, 2f, 1f);
//                    }
//                }
//                else if (sdlEvent.key.keysym.sym == SDL.SDL_Keycode.SDLK_KP_2)
//                {
//                    ICameraOrtho cam = GameEngine.ActiveScene.Components.OfType<ICameraOrtho>().FirstOrDefault();
//                    if (cam != null)
//                    {
//                        foreach (IDrawable camDrawable in cam.Drawables)
//                        {
//                            camDrawable.Transform.Position /= new Vector3(2f, 2f, 1f);
//                        }
//                        cam.Transform.Scale /= new Vector3(2f, 2f, 1f);
//                    }
//                }
//                else if (sdlEvent.key.keysym.sym == SDL.SDL_Keycode.SDLK_w)
//                {
//                    ITextSprite textSprite = GameEngine.ActiveScene.Children
//                        .FirstOrDefault(go => go != _debugDrawerGo && go.Components.OfType<ITextSprite>().Any())?
//                        .Components.OfType<ITextSprite>()
//                        .FirstOrDefault();
//                    if (textSprite != null)
//                    {
//                        textSprite.Transform.PositionByRef().Y -= _debugCameraOrtho.ViewSpaceBBox.Y / 2f;
//                    }
//                }
//                else if (sdlEvent.key.keysym.sym == SDL.SDL_Keycode.SDLK_s)
//                {
//                    ITextSprite textSprite = GameEngine.ActiveScene.Children
//                        .FirstOrDefault(go => go != _debugDrawerGo && go.Components.OfType<ITextSprite>().Any())?
//                        .Components.OfType<ITextSprite>()
//                        .FirstOrDefault();
//                    if (textSprite != null)
//                    {
//                        textSprite.Transform.PositionByRef().Y += _debugCameraOrtho.ViewSpaceBBox.Y / 2f;
//                    }
//                }
//                else if (sdlEvent.key.keysym.sym == SDL.SDL_Keycode.SDLK_a)
//                {
//                    ITextSprite textSprite = GameEngine.ActiveScene.Children
//                        .FirstOrDefault(go => go != _debugDrawerGo && go.Components.OfType<ITextSprite>().Any())?
//                        .Components.OfType<ITextSprite>()
//                        .FirstOrDefault();
//                    if (textSprite != null)
//                    {
//                        textSprite.Transform.PositionByRef().X -= _debugCameraOrtho.ViewSpaceBBox.X / 2f;
//                    }
//                }
//                else if (sdlEvent.key.keysym.sym == SDL.SDL_Keycode.SDLK_d)
//                {
//                    ITextSprite textSprite = GameEngine.ActiveScene.Children
//                        .FirstOrDefault(go => go != _debugDrawerGo && go.Components.OfType<ITextSprite>().Any())?
//                        .Components.OfType<ITextSprite>()
//                        .FirstOrDefault();
//                    if (textSprite != null)
//                    {
//                        textSprite.Transform.PositionByRef().X += _debugCameraOrtho.ViewSpaceBBox.X / 2f;
//                    }
//                }
//                else if (sdlEvent.key.keysym.sym == SDL.SDL_Keycode.SDLK_q)
//                {
//                    ITextSprite textSprite = GameEngine.ActiveScene.Children
//                        .FirstOrDefault(go => go != _debugDrawerGo && go.Components.OfType<ITextSprite>().Any())?
//                        .Components.OfType<ITextSprite>()
//                        .FirstOrDefault();
//                    if (textSprite != null)
//                    {
//                        int previousValue = (int) textSprite.AlignmentHorizontal;
//                        if (previousValue >= (int) TextAlignmentHorizontal.Right)
//                        {
//                            textSprite.AlignmentHorizontal = TextAlignmentHorizontal.Left;
//                        }
//                        else
//                        {
//                            textSprite.AlignmentHorizontal = (TextAlignmentHorizontal) previousValue + 1;
//                        }
//                    }
//                }
//                else if (sdlEvent.key.keysym.sym == SDL.SDL_Keycode.SDLK_e)
//                {
//                    ITextSprite textSprite = GameEngine.ActiveScene.Children
//                        .FirstOrDefault(go => go != _debugDrawerGo && go.Components.OfType<ITextSprite>().Any())?
//                        .Components.OfType<ITextSprite>()
//                        .FirstOrDefault();
//                    if (textSprite != null)
//                    {
//                        int previousValue = (int) textSprite.AlignmentVertical;
//                        if (previousValue >= (int) TextAlignmentVertical.Bottom)
//                        {
//                            textSprite.AlignmentVertical= TextAlignmentVertical.Top;
//                        }
//                        else
//                        {
//                            textSprite.AlignmentVertical = (TextAlignmentVertical) previousValue + 1;
//                        }
//                    }
//                }
//#endif
//            }
//        }

        private void OnResize(Vector2 size)
        {
            UpdateViewport(size);
        }

        public static void OnGlDebugMessage(DebugSource source, DebugType type, uint id, DebugSeverity severity, int length, IntPtr message, IntPtr userParam)
        {
            string msg = $"{Marshal.PtrToStringAnsi(message, length)} | id:{id} severity:{severity} type:{type} source:{source}";

            LogSeverity logSeverity = severity switch
            {
                DebugSeverity.DebugSeverityLow when type != DebugType.DebugTypePerformance => LogSeverity.Warning,
                DebugSeverity.DebugSeverityMedium when type != DebugType.DebugTypePerformance => LogSeverity.Error,
                DebugSeverity.DebugSeverityHigh when type != DebugType.DebugTypePerformance => LogSeverity.CriticalError,
                _ => LogSeverity.Debug
            };

            GameEngine.LogAppendLine(logSeverity, "OpenGLNative", msg);
        }

        private void RunRenderables(float deltaTime)
        {
            List<IRenderUpdateable> validRenderables = GameEngine.Renderables
                .Where(renderable =>
                {
                    if (!renderable.Active)
                    {
                        return false;
                    }

                    return renderable switch
                    {
                        IParentable<IGameObject> parentable when parentable.Parent != null => true,
                        IParentable<IScene> sceneParentable when sceneParentable.Parent != null => true,
                        _ => false
                    };
                })
                .ToList();

            Task[] renderTasks = new Task[validRenderables.Count];
            for (int i = 0; i < validRenderables.Count; i++)
            {
                IRenderUpdateable renderable = validRenderables[i];
                Task renderTask = EngineThread.ExecuteOnThreadAsync(() => renderable.RenderUpdate(deltaTime));
                renderTasks[i] = renderTask;
            }
            Task.WaitAll(renderTasks.ToArray());
        }

        private void RenderScene()
        {
            if (_geometryPerspFrameBuffer == null || _geometryOrthoFrameBuffer == null) return;
            if (!_geometryPerspFrameBuffer.Active || !_geometryOrthoFrameBuffer.Active) return;

            List<ICameraPersp> perspectiveCameras = Cameras.OfType<ICameraPersp>().Where(camera => !camera.Destroyed && camera.Active).ToList();
            foreach (ICameraPersp cameraPersp in perspectiveCameras)
            {
                cameraPersp.DrawGeometry();
            }

            List<ICameraOrtho> orthoCameras = Cameras.OfType<ICameraOrtho>().Where(camera => !camera.Destroyed && camera.Active).ToList();
            foreach (ICameraOrtho cameraOrtho in orthoCameras)
            {
                cameraOrtho.DrawGeometry();
            }

            DepthTestEnabled = false;
            _geometryPerspFrameBuffer.Bind(true);
            foreach (ICameraPersp cameraPersp in perspectiveCameras.Where(c => c.RenderToScreen))
            {
                cameraPersp.FrameBuffers.LastOrDefault()?.Draw(cameraPersp, Matrix4.Identity, Matrix4.Identity);
            }
            _geometryPerspFrameBuffer.Unbind();

            _geometryOrthoFrameBuffer.Bind(true);
            foreach (ICameraOrtho cameraOrtho in orthoCameras.Where(c => c.RenderToScreen))
            {
                cameraOrtho.FrameBuffers.LastOrDefault()?.Draw(cameraOrtho, Matrix4.Identity, Matrix4.Identity);
            }
            _geometryOrthoFrameBuffer.Unbind();

            DrawGeometryFrameBuffers();
        }

        private void DrawGeometryFrameBuffers()
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            _geometryPerspFrameBuffer.Draw(null, Matrix4.Identity, Matrix4.Identity);
            _geometryOrthoFrameBuffer.Draw(null, Matrix4.Identity, Matrix4.Identity);
        }
    }
}