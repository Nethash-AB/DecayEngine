using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using CommandLine;
using DecayEngine.Core;
using DecayEngine.NativeJS;
using DecayEngine.ModuleSDK;
using DecayEngine.OpenGL;
using DecayEngine.SDL2.Native.Input;
using DecayEngine.Tween;
using DecayEngine.TypingsGenerator.Model;

namespace DecayEngine.TypingsGenerator
{
    public static class Program
    {
        public static Dictionary<string, Assembly> DecayEngineModules;
        public static void Main(string[] args)
        {
            Parser.Default.ParseArguments<CommandLineOptions>(args)
                .WithParsed(options =>
                {
                    Console.WriteLine("(Stage 1/5) Loading Assemblies.");
                    LoadAssemblies();
                    Console.WriteLine("Assemblies Loaded.");
                    Global global = new Global();

                    TypeReflector reflector = new TypeReflector();
                    global.AddRange(reflector.ReflectExports());

                    string outputFilePath = Path.Combine(options.OutputPath, "global.d.ts");
                    Console.WriteLine($"\n(Stage 5/5) Writing typings to disc => {outputFilePath}.");

                    if (!Directory.Exists(options.OutputPath))
                    {
                        Directory.CreateDirectory(options.OutputPath);
                    }
                    using (TextWriter writer = new StreamWriter(outputFilePath))
                    {
                        writer.WriteLine(global.ToString());
                    }

                    Console.WriteLine("Typings written to disk successfully.");
                })
                .WithNotParsed(errors =>
                {
                    foreach (Error error in errors)
                    {
                        Console.Error.WriteLine(error.ToString());
                    }
                });
        }

        private static void LoadAssemblies()
        {
            DecayEngineModules = new Dictionary<string, Assembly>
            {
                ["Core"] = typeof(GameEngineImpl).Assembly,
                ["NativeJS"] = typeof(JavascriptEngine).Assembly,
                ["ModuleSDK"] = typeof(GameEngine).Assembly,
                ["OpenGL"] = typeof(OpenGlEngine).Assembly,
                ["SDL2"] = typeof(Sdl2InputProvider).Assembly,
                ["TweenExtension"] = typeof(TweenExtension).Assembly
            };

            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                LoadReferencedAssemblies(assembly);
            }
        }

        private static void LoadReferencedAssemblies(Assembly assembly)
        {
            foreach (AssemblyName name in assembly.GetReferencedAssemblies())
            {
                if (AppDomain.CurrentDomain.GetAssemblies().All(a => a.FullName != name.FullName))
                {
                    LoadReferencedAssemblies(Assembly.Load(name));
                }
            }
        }
    }
}