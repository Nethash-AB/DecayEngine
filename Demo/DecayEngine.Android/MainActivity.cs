using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Android.App;
using Android.Content.PM;
using Android.Util;
using Android.Views;
using DecayEngine.Bullet.Managed;
using DecayEngine.Core;
using DecayEngine.DecPakLib.Bundle;
using DecayEngine.DecPakLib.Math.Vector;
using DecayEngine.DecPakLib.Package;
using DecayEngine.DecPakLib.Resource.RootElement.Font;
using DecayEngine.DecPakLib.Resource.RootElement.Shader;
using DecayEngine.DecPakLib.Resource.RootElement.ShaderProgram;
using DecayEngine.Fmod.Android;
using DecayEngine.Fmod.Managed;
using DecayEngine.NativeJS;
using DecayEngine.ModuleSDK;
using DecayEngine.ModuleSDK.Engine.Physics;
using DecayEngine.ModuleSDK.Logging;
using DecayEngine.OpenGL;
using DecayEngine.SDL2.Android;
using DecayEngine.Tween;
using ProtoBuf;
using Thread = System.Threading.Thread;

namespace DecayEngine.Android
{
    [Activity(
        Label = "@string/ApplicationName",
        MainLauncher = true,
        HardwareAccelerated = true,
        ScreenOrientation = ScreenOrientation.Landscape,
        ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize,
        Icon = "@drawable/icon",
        RoundIcon = "@drawable/icon",
        Immersive = true
    )]
    public class MainActivity : SDLActivity
    {
        [DllImport("main")]
        private static extern void SetMain(Main main);
        private delegate void Main();

        private Main _mainFunc;

        private static MainActivity Instance { get; set; }
        private static ResourceBundle Bundle { get; set; }

        public override void LoadLibraries()
        {
            base.LoadLibraries();
#if DEBUG
            FmodAndroid.LoadDebug(this);
#else
            FmodAndroid.LoadRelease(this);
#endif

            _mainFunc = Init;
            SetMain(_mainFunc);
        }

        public override void OnWindowFocusChanged(bool hasFocus) {
            base.OnWindowFocusChanged(hasFocus);

            if (!hasFocus)
            {
            }
            else
            {
                RunOnUiThread(ActionBar.Hide);
                RunOnUiThread(SetImmersive);
            }
        }

        protected override void OnStart()
        {
            base.OnStart();
            Instance = this;
            ActionBar.Hide();
            SetImmersive();

            if (Bundle == null) return;

            foreach (DataPackage pkg in Bundle.Packages)
            {
                pkg.PackageStreamer = new AndroidAssetPackageStreamer(Assets);
            }
        }

        protected override void OnDestroy()
        {
            GameEngine.Shutdown();
            FmodAndroid.Close();
            base.OnDestroy();
        }

        private void SetImmersive()
        {
            Window.DecorView.SystemUiVisibility = (StatusBarVisibility) (
                SystemUiFlags.LayoutStable |
                SystemUiFlags.LayoutHideNavigation |
                SystemUiFlags.LayoutFullscreen |
                SystemUiFlags.HideNavigation |
                SystemUiFlags.Fullscreen |
                SystemUiFlags.ImmersiveSticky
            );
        }

        public static void Init()
        {
            using (Stream bundleStream = Instance.Assets.Open("Resources.decmeta"))
            {
                Bundle = Serializer.Deserialize<ResourceBundle>(bundleStream);
            }

            foreach (DataPackage pkg in Bundle.Packages)
            {
                pkg.PackageStreamer = new AndroidAssetPackageStreamer(Instance.Assets);
            }

            Thread.GetDomain().UnhandledException += (_, __) => GameEngine.Shutdown();

            DisplayMetrics dm = new DisplayMetrics();
            Instance.WindowManager.DefaultDisplay.GetMetrics(dm);

            try
            {
                GameEngine.Init<GameEngineImpl>(builder => builder
                    .AddResourceBundle(Bundle)
                    .UseScriptEngine<JavascriptEngine, JavascriptEngineOptions>()
                    .UseRenderEngine<OpenGlEngine, OpenGlEngineOptions>(engineOptions =>
                    {
                        engineOptions.GameSurface = new Sdl2AndroidGlSurface(
                            "Decay Engine",
                            true,
                            new Vector2(dm.WidthPixels, dm.HeightPixels),
                            false);

                        engineOptions.IsEmbedded = true;
                        engineOptions.UseGeometryShaders = true;
                        engineOptions.UseCompressedTextures = false;
                        engineOptions.UseSpirvShaders = false;
                        engineOptions.FallbackShaderLanguage = ShaderLanguage.GlslEs;

                        engineOptions.DrawDebug = true;
                        engineOptions.DrawDebugNames = true;
                        engineOptions.DrawDebugOriginCrosshair = true;
                        engineOptions.DebugGeometryShaderProgram =
                            Bundle.Resources.OfType<ShaderProgramResource>().FirstOrDefault(res => res.Id == "default_shader_program_wireframe");
                        engineOptions.DebugLinesShaderProgram =
                            Bundle.Resources.OfType<ShaderProgramResource>().FirstOrDefault(res => res.Id == "default_debug_shader_lines");
                        engineOptions.DebugTextShaderProgram =
                            Bundle.Resources.OfType<ShaderProgramResource>().FirstOrDefault(res => res.Id == "default_text_shader_program");
                        engineOptions.DebugTextFont =
                            Bundle.Resources.OfType<FontResource>().FirstOrDefault(res => res.Id == "test_font");

                    })
                    .UseSoundEngine<FmodEngine, FmodEngineOptions>()
                    .UsePhysicsEngine<BulletEngine, BulletEngineOptions>(engineOptions =>
                    {
                        engineOptions.DrawDebug = true;
                    })
                    .AddExtension<TweenExtension, TweenExtensionOptions>()
                    .AddLogger<AndroidLogcatLogger, AndroidLogcatLoggerOptions>(loggerOptions =>
                    {
                        loggerOptions.MinimumSeverity = LogSeverity.Info;
                    })
                    .WithInitScene(Instance.PackageManager.GetApplicationInfo(Instance.PackageName, PackageInfoFlags.MetaData).MetaData.GetString("InitScene"))
                    .Build()).Wait();
            }
            catch (Exception e)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine($"DecayEngine experienced an unrecoverable {e.GetType()}: {e.Message}");
                if (e.StackTrace != null)
                {
                    sb.AppendLine("Stack Trace:");
                    sb.AppendLine(e.StackTrace);
                }
                if (e.InnerException != null)
                {
                    sb.AppendLine($"Inner Exception: {e.InnerException.GetType()}: {e.InnerException.Message}");
                    sb.AppendLine("Stack Trace:");
                    sb.AppendLine(e.InnerException.StackTrace);
                }

                Log.Error(AndroidConstants.LogcatTag, sb.ToString());
            }
        }

//        public override void OnConfigurationChanged(Configuration newConfig)
//        {
//            base.OnConfigurationChanged(newConfig);
//        }

//        private GameWindow _gameWindow;
//        private readonly Delegates.DebugProc _onGlDebugMessage = OnGlDebugMessage;
//        private Main _mainFunc;
//
//        public static void OnGlDebugMessage(DebugSource source, DebugType type, int id, DebugSeverity severity, int length, IntPtr message, IntPtr userParam)
//        {
//            string msg = $"{Marshal.PtrToStringAnsi(message, length)} | id:{id} severity:{severity} type:{type} source:{source}";
//            if (severity == DebugSeverity.DebugSeverityLow || severity == DebugSeverity.DebugSeverityMedium || severity == DebugSeverity.DebugSeverityHigh)
//            {
//                System.Diagnostics.Debug.WriteLine($"[OpenGLNative][Warning]: {msg}");
//            }
//            else
//            {
//                System.Diagnostics.Debug.WriteLine($"[OpenGLNative][Debug]: {msg}");
//            }
//        }
//
//        protected override void OnDestroy()
//        {
//            _gameWindow.Quit();
//        }
//
//        public override void LoadLibraries()
//        {
//            base.LoadLibraries();
//
//            DisplayMetrics dm = new DisplayMetrics();
//            WindowManager.DefaultDisplay.GetMetrics(dm);
//
//            _gameWindow = new GameWindow(null, "DecayEngine Android Alpha", dm.WidthPixels, dm.HeightPixels,
//                true, true, false, false,
//                OpenGlFlavour.Embedded, 2, 0, true
//            )
//            {
//                OnInit = OnInit,
//                OnResize = OnResize,
//                OnEvent = OnEvent,
//                OnRender = OnRender,
//                OnQuit = OnQuit
//            };
//
//            _mainFunc = InitGameWindow;
//            SetMain(_mainFunc);
//
//            System.Diagnostics.Debug.WriteLine("TESTESTESTESTONCREATE");
//
//            RunOnUiThread(ActionBar.Hide);
//            RunOnUiThread(SetImmersive);
//
//            System.Diagnostics.Debug.WriteLine("TEST4");
////            while (!_gameWindow.Running)
////            {
////                Task.Delay(500);
////            }
//        }
//
//        public override void OnWindowFocusChanged(bool hasFocus) {
//            base.OnWindowFocusChanged(hasFocus);
//
//            if (!hasFocus)
//            {
////                _gameWindow.Pause();
//            }
//            else
//            {
////                _gameWindow.Resume();
//
//                if (_gameWindow.FullScreen)
//                {
//                    RunOnUiThread(ActionBar.Hide);
//                    RunOnUiThread(SetImmersive);
//                }
//            }
//        }
//
//        public void SetImmersive()
//        {
//            Window.DecorView.SystemUiVisibility = (StatusBarVisibility) (
//                SystemUiFlags.LayoutStable |
//                SystemUiFlags.LayoutHideNavigation |
//                SystemUiFlags.LayoutFullscreen |
//                SystemUiFlags.HideNavigation |
//                SystemUiFlags.Fullscreen |
//                SystemUiFlags.ImmersiveSticky
//            );
//        }
//
//        public void InitGameWindow()
//        {
//            System.Diagnostics.Debug.WriteLine("INITGAMEWINDOW1");
//
//            System.Diagnostics.Debug.WriteLine("INITGAMEWINDOW2");
//            _gameWindow.Run();
//            System.Diagnostics.Debug.WriteLine("INITGAMEWINDOW3");
//        }
//
//        private void OnInit()
//        {
//            InitGl();
//        }
//
//        private void OnResize()
//        {
//            UpdateViewport();
//        }
//
//        private void OnEvent(SDL.SDL_Event e)
//        {
//        }
//
//        private void OnRender(float deltaTime)
//        {
//            System.Diagnostics.Debug.WriteLine("ONRENDER");
//            GL.ClearColor(1f, 0f, 0f, 1f);
//            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
//        }
//
//        private void OnQuit()
//        {
//            base.OnDestroy();
//        }
//
//        private void InitGl()
//        {
//            System.Diagnostics.Debug.WriteLine("INITGL1");
//
//            GL.Enable(EnableCap.DebugOutput);
//            GL.DebugMessageCallback(_onGlDebugMessage, IntPtr.Zero);
//
//            GL.Enable(EnableCap.Multisample);
//            GL.Enable(EnableCap.CullFace);
//
//            GL.Enable(EnableCap.Blend);
//            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
//
//            System.Diagnostics.Debug.WriteLine("INITGL2");
//
//            UpdateViewport();
//        }
//
//        private void UpdateViewport()
//        {
//            GL.Viewport(0, 0, _gameWindow.Width, _gameWindow.Height);
//        }
//
//        private delegate void Main();
//
//        [DllImport("main")]
//        private static extern void SetMain(Main main);
    }
}