using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;
using System.Threading.Tasks;
using CommandLine;
using DecayEngine.Bullet.Managed;
using DecayEngine.Core;
using DecayEngine.DecPakLib.Bundle;
using DecayEngine.DecPakLib.Loader;
using DecayEngine.DecPakLib.Math.Vector;
using DecayEngine.DecPakLib.Package;
using DecayEngine.DecPakLib.Resource.RootElement.Font;
using DecayEngine.DecPakLib.Resource.RootElement.Shader;
using DecayEngine.DecPakLib.Resource.RootElement.ShaderProgram;
using DecayEngine.Fmod.Managed;
using DecayEngine.NativeJS;
using DecayEngine.ModuleSDK;
using DecayEngine.ModuleSDK.Engine.Physics;
using DecayEngine.ModuleSDK.Logging;
using DecayEngine.ModuleSDK.Logging.DebugConsole;
using DecayEngine.OpenGL;
using DecayEngine.SDL2.Desktop;
using DecayEngine.SDL2.Native.Input;
using DecayEngine.Tween;
using ProtoBuf;

namespace DecayEngine.Standalone
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                Parser.Default.ParseArguments<CommandLineOptions>(args)
                    .WithParsed(options =>
                    {
                        ResourceBundle bundle;
                        using (FileStream fs = File.OpenRead(options.BundlePath))
                        {
                            bundle = Serializer.Deserialize<ResourceBundle>(fs);
                        }

                        Uri inputUri = new Uri(options.BundlePath, UriKind.RelativeOrAbsolute);
                        if (!inputUri.IsAbsoluteUri)
                        {
                            inputUri = new Uri(
                                new Uri($"{Directory.GetCurrentDirectory()}{Path.DirectorySeparatorChar}", UriKind.Absolute),
                                inputUri
                            );
                        }

                        foreach (DataPackage pkg in bundle.Packages)
                        {
                            pkg.PackageStreamer =
                                new FilePackageStreamer(
                                    new Uri($"{Path.GetDirectoryName(inputUri.LocalPath)}{Path.DirectorySeparatorChar}",
                                        UriKind.Absolute
                                    )
                                );
                        }

                        AppDomain.CurrentDomain.ProcessExit += (_, __) => GameEngine.Shutdown();
                        AssemblyLoadContext.Default.Unloading += _ => GameEngine.Shutdown();
                        AppDomain.CurrentDomain.UnhandledException += (sender, eventArgs) => throw ((Exception) eventArgs.ExceptionObject);

                        Task engineTask = GameEngine.Init<GameEngineImpl>(builder => builder
                            .AddResourceBundle(bundle)
                            .UseScriptEngine<JavascriptEngine, JavascriptEngineOptions>()
                            .UseRenderEngine<OpenGlEngine, OpenGlEngineOptions>(engineOptions =>
                            {
                                engineOptions.GameSurface = new Sdl2DesktopGlWindow(
                                    "Decay Engine",
                                    true,
                                    new Vector2(1600, 900),
                                    false,
                                    GetDefaultIcon());

                                engineOptions.IsEmbedded = false;
                                engineOptions.UseGeometryShaders = true;
                                engineOptions.UseCompressedTextures = true;
                                engineOptions.UseSpirvShaders = false;
                                engineOptions.FallbackShaderLanguage = ShaderLanguage.Glsl;

                                engineOptions.DefaultPostProcessingShaderProgram =
                                    bundle.Resources.OfType<ShaderProgramResource>().FirstOrDefault(res => res.Id == "default_framebuffer_shader_program");
                                engineOptions.DefaultPbrLightingShaderProgram =
                                    bundle.Resources.OfType<ShaderProgramResource>().FirstOrDefault(res => res.Id == "default_deferred_lighting_shader_program");

                                engineOptions.DrawDebug = true;
                                engineOptions.DrawDebugNames = false;
                                engineOptions.DrawDebugOriginCrosshair = false;
                                engineOptions.DebugGeometryShaderProgram =
                                    bundle.Resources.OfType<ShaderProgramResource>().FirstOrDefault(res => res.Id == "debug_geometry_shader_program");
                                engineOptions.DebugLinesShaderProgram =
                                    bundle.Resources.OfType<ShaderProgramResource>().FirstOrDefault(res => res.Id == "debug_lines_shader_program");
                                engineOptions.DebugTextShaderProgram =
                                    bundle.Resources.OfType<ShaderProgramResource>().FirstOrDefault(res => res.Id == "debug_text_shader_program");
                                engineOptions.DebugTextFont =
                                    bundle.Resources.OfType<FontResource>().FirstOrDefault(res => res.Id == "test_font");

                            })
                            .UseSoundEngine<FmodEngine, FmodEngineOptions>()
                            .UsePhysicsEngine<BulletEngine, BulletEngineOptions>(engineOptions =>
                            {
                                engineOptions.DrawDebug = true;
                            })
                            .AddInputProvider<Sdl2InputProvider, Sdl2InputProviderOptions>()
                            .AddExtension<TweenExtension, TweenExtensionOptions>()
                            .AddLogger<ConsoleLogger, ConsoleLoggerOptions>(loggerOptions =>
                            {
                                loggerOptions.MinimumSeverity = LogSeverity.Info;
                                loggerOptions.OutputPaths = true;
                                loggerOptions.UseDebugStream = false;
                            })
                            .WithInitScene(options.InitSceneId)
                            .Build());

//                        GameEngine.OnScenePostload += (scene, b) =>
//                        {
//                            if (scene.Name == "test_maya_export_scene")
//                            {
//                                MayaTestSceneDemoScript.Run();
//                            }
//                        };

                        engineTask.Wait();
                    })
                    .WithNotParsed(errors =>
                    {
                        foreach (Error error in errors)
                        {
                            Console.Error.WriteLine(error.ToString());
                        }
                    });
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

                Console.WriteLine(sb.ToString());

                string minidumpDirPath = Path.Combine(Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory)
                    .Replace($"file:{Path.DirectorySeparatorChar}", ""), "dumps");

                string windir = Environment.GetEnvironmentVariable("windir"); // https://stackoverflow.com/a/38795621 Weird flex but ok.
                if (!string.IsNullOrEmpty(windir) && windir.Contains(@"\") && Directory.Exists(windir))
                {
                    WindowsCrashHandler.ExceptionHandler(minidumpDirPath);
                }

                GameEngine.Shutdown();
            }
        }

        private static Rgba32Icon GetDefaultIcon()
        {
            const string iconName = "window_icon.png";
            Assembly entryAssembly = Assembly.GetEntryAssembly();
            if (entryAssembly != null)
            {
                foreach (string resourceName in entryAssembly.GetManifestResourceNames())
                {
                    if (!resourceName.Contains(iconName)) continue;

                    return new Rgba32Icon(entryAssembly.GetManifestResourceStream(resourceName));
                }
            }

            Assembly thisAssembly = Assembly.GetExecutingAssembly();
            Stream iconStream = thisAssembly.GetManifestResourceNames()
                .Where(resourceName => resourceName.Contains(iconName))
                .Select(resourceName => thisAssembly.GetManifestResourceStream(resourceName))
                .FirstOrDefault();

            return iconStream != null ? new Rgba32Icon(iconStream) : null;
        }
    }
}