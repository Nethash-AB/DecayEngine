using DecayEngine.Core;
using DecayEngine.DecPakLib.Bundle;
using DecayEngine.DecPakLib.Package;
using DecayEngine.DecPakLib.Resource.RootElement.Font;
using DecayEngine.DecPakLib.Resource.RootElement.Shader;
using DecayEngine.DecPakLib.Resource.RootElement.ShaderProgram;
using DecayEngine.NativeJS;
using DecayEngine.ModuleSDK;
using DecayEngine.ModuleSDK.Logging;
using DecayEngine.ModuleSDK.Logging.DebugConsole;
using DecayEngine.OpenGL;
using DecayEngine.SDL2.WinRt;
using DecayEngine.Tween;
using ProtoBuf;
using System;
using System.IO;
using System.Linq;
using Windows.Storage;
using Windows.Storage.Streams;
using DecayEngine.Bullet.Managed;
using DecayEngine.DecPakLib.Math.Vector;
using DecayEngine.Fmod.Managed;
using DecayEngine.ModuleSDK.Engine.Physics;

namespace DecayEngine.Demo.Xbox
{
    public class App
    {
        private static ResourceBundle Bundle { get; set; }

        [MTAThread]
        public static void Main(string[] args)
        {
            Uri resourceBundleUri = new Uri("ms-appx:///Resources.decmeta");
            StorageFile resourceBundleFile = StorageFile.GetFileFromApplicationUriAsync(resourceBundleUri).AsTask().Result;
            IRandomAccessStreamWithContentType bundleWinStream = resourceBundleFile.OpenReadAsync().AsTask().Result;
            using (Stream bundleStream = bundleWinStream.GetInputStreamAt(0).AsStreamForRead())
            {
                Bundle = Serializer.Deserialize<ResourceBundle>(bundleStream);
            }

            foreach (DataPackage pkg in Bundle.Packages)
            {
                pkg.PackageStreamer = new WinRtAssetPackageStreamer(new Uri("ms-appx:///"));
            }

            GameEngine.Init<GameEngineImpl>(builder => builder
                .AddResourceBundle(Bundle)
                .UseScriptEngine<JavascriptEngine, JavascriptEngineOptions>()
                .UseRenderEngine<OpenGlEngine, OpenGlEngineOptions>(engineOptions =>
                {
                    engineOptions.GameSurface = new Sdl2WinRtGlWindow(
                        "Decay Engine",
                        true,
                        new Vector2(0, 0),
                        false,
                        240);

                    engineOptions.IsEmbedded = true;
                    engineOptions.UseGeometryShaders = false;
                    engineOptions.UseCompressedTextures = true;
                    engineOptions.UseSpirvShaders = false;
                    engineOptions.FallbackShaderLanguage = ShaderLanguage.GlslEs;

                    engineOptions.DrawDebug = true;
                    engineOptions.DrawDebugNames = false;
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
                .AddLogger<ConsoleLogger, ConsoleLoggerOptions>(loggerOptions =>
                {
                    loggerOptions.MinimumSeverity = LogSeverity.Info;
                    loggerOptions.OutputPaths = true;
                    loggerOptions.UseDebugStream = true;
                })
                .WithInitScene("test_scene_car")
                .Build()).Wait();
        }
    }
}