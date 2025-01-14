using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Assimp;
using CommandLine;
using DecayEngine.DecPakLib.Bundle;
using DecayEngine.DecPakLib.Platform;
using DecayEngine.ResourceBuilder.CommandLine;
using DecayEngine.ResourceBuilderLib.Resource;

namespace DecayEngine.ResourceBuilder
{
    public static class Program
    {
        public static int Main(string[] args)
        {
            Parser parser = new Parser(settings =>
            {
                settings.AutoHelp = true;
                settings.AutoVersion = true;
                settings.CaseSensitive = false;
                settings.IgnoreUnknownArguments = false;
                settings.CaseInsensitiveEnumValues = true;
                settings.HelpWriter = Console.Out;
                settings.MaximumDisplayWidth = Console.BufferWidth;
            });

            return parser.ParseArguments<BuildOptions, ExtractOptions, NewOptions, ListPlatformsOptions>(args)
                .MapResult(
                    (BuildOptions opts) => Build(opts),
                    (ExtractOptions opts) => Extract(opts),
                    (NewOptions opts) => CreateNewProject(opts),
                    (ListPlatformsOptions _) => ListPlatforms(),
                    errs => 1);
        }

        private static int Build(BuildOptions opts)
        {
            try
            {
                Uri inputUri = new Uri(opts.InputPath + @"\", UriKind.RelativeOrAbsolute);
                Uri outputUri = new Uri(opts.OutputPath + @"\", UriKind.RelativeOrAbsolute);
                if (opts.TargetPlatforms == null) opts.TargetPlatforms = new List<TargetPlatform> {TargetPlatform.All};

                Console.WriteLine($"---Building Platforms: {string.Join(", ", opts.TargetPlatforms.Select(p => p.ToString()))}---");

                foreach (TargetPlatform targetPlatform in opts.TargetPlatforms)
                {
                    Console.WriteLine($"\n---Building Platform: {targetPlatform.ToString()}---");

                    Console.WriteLine("\n---Deserialization Phase---");
                    ResourceBundle bundle = ResourceSerializationController.Deserialize(inputUri, targetPlatform);

                    Console.WriteLine("\n---Compilation Phase---");
                    Uri platformSubDirUri = new Uri($"./{targetPlatform.ToString()}/", UriKind.Relative);
                    Uri platformOutputUri = new Uri(outputUri, platformSubDirUri);
                    ResourceCompilationController.Compile(bundle, inputUri, platformOutputUri);

                    Console.WriteLine($"\n---Done Building Platform: {targetPlatform.ToString()}---");
                }

                Console.WriteLine("\n---Done---");

                return 0;
            }
            catch (Exception e)
            {
#if DEBUG
                Console.WriteLine(e.ToString());
#else
                Console.WriteLine(e.Message);
#endif
                return 1;
            }
        }

        private static int Extract(ExtractOptions opts)
        {
            try
            {
                Uri inputUri = new Uri(opts.InputPath);
                Uri outputUri = new Uri(opts.OutputPath);

                Console.WriteLine("---Loading Phase---");
                ResourceBundle bundle = ResourceLoadingController.Load(inputUri);
                Console.WriteLine("\n---Deserialization Phase---");
                ResourceSerializationController.Serialize(bundle, outputUri);
                Console.WriteLine("\n---Extraction Phase---");
                ResourceCompilationController.Extract(bundle, inputUri, outputUri);
                Console.WriteLine("---Done---");

                return 0;
            }
            catch (Exception e)
            {
#if DEBUG
                Console.WriteLine(e.ToString());
#else
                Console.WriteLine(e.Message);
#endif
                return 1;
            }
        }

        private static int CreateNewProject(NewOptions opts)
        {
            try
            {
                string path = Path.IsPathFullyQualified(opts.Path) ? opts.Path : Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), opts.Path));

                if (!Directory.Exists(path))
                {
                    Console.WriteLine($"Creating directory: {path}");
                    Directory.CreateDirectory(path);
                }

                string package = TypescriptUtils.GetPackage();
                string tsConfig = TypescriptUtils.GetTsConfig();
                string tsLint = TypescriptUtils.GetTsLint();
                string gitIgnore = TypescriptUtils.GetGitIgnore();

                Console.WriteLine("Writing package.json.");
                string packagePath = Path.Combine(path, "package.json");
                if (File.Exists(packagePath) && opts.Clean)
                {
                    File.Delete(packagePath);
                }
                using (TextWriter writer = new StreamWriter(packagePath))
                {
                    writer.WriteLine(package);
                }

                Console.WriteLine("Writing tsconfig.json.");
                string tsConfigPath = Path.Combine(path, "tsconfig.json");
                if (File.Exists(tsConfigPath) && opts.Clean)
                {
                    File.Delete(tsConfigPath);
                }
                using (TextWriter writer = new StreamWriter(tsConfigPath))
                {
                    writer.WriteLine(tsConfig);
                }

                Console.WriteLine("Writing tslint.json.");
                string tsLintPath = Path.Combine(path, "tslint.json");
                if (File.Exists(tsLintPath) && opts.Clean)
                {
                    File.Delete(tsLintPath);
                }
                using (TextWriter writer = new StreamWriter(tsLintPath))
                {
                    writer.WriteLine(tsLint);
                }

                Console.WriteLine("Writing .gitignore.");
                string gitIgnorePath = Path.Combine(path, ".gitignore");
                if (File.Exists(gitIgnorePath) && opts.Clean)
                {
                    File.Delete(gitIgnorePath);
                }
                using (TextWriter writer = new StreamWriter(gitIgnorePath))
                {
                    writer.WriteLine(gitIgnore);
                }

                Console.WriteLine("Installing NPM modules.");
                string npmModulesPath = Path.Combine(path, "node_modules");
                if (Directory.Exists(npmModulesPath) && opts.Clean)
                {
                    Directory.Delete(npmModulesPath, true);
                }
                TypescriptUtils.InstallNpmModules(path);

                Console.WriteLine("\n---Done---");

                return 0;
            }
            catch (Exception e)
            {
#if DEBUG
                Console.WriteLine(e.ToString());
#else
                Console.WriteLine(e.Message);
#endif
                return 1;
            }
        }

        private static int ListPlatforms()
        {
            Console.WriteLine("Individual platforms:");
            Console.WriteLine($"- {nameof(TargetPlatform.WindowsNative)} - Windows Win32.");
            Console.WriteLine($"- {nameof(TargetPlatform.WindowsUwp)} - Windows 10 UWP (unsupported).");
            Console.WriteLine($"- {nameof(TargetPlatform.Linux)} - Linux.");
            Console.WriteLine($"- {nameof(TargetPlatform.MacOs)} - MacOS.");
            Console.WriteLine($"- {nameof(TargetPlatform.Android)} - Android.");
            Console.WriteLine($"- {nameof(TargetPlatform.AppleIOs)} - iOS.");
            Console.WriteLine($"- {nameof(TargetPlatform.Switch)} - Nintendo Switch.");
            Console.WriteLine($"- {nameof(TargetPlatform.Xbox)} - Xbox One.");
            Console.WriteLine($"- {nameof(TargetPlatform.Playstation4)} - Sony Playstation 4.");

            Console.WriteLine("\nAlias platforms:");
            Console.WriteLine($"- {nameof(TargetPlatform.Windows)} - {nameof(TargetPlatform.WindowsNative)} + {nameof(TargetPlatform.WindowsUwp)}.");
            Console.WriteLine(
                $"- {nameof(TargetPlatform.Desktop)} - {nameof(TargetPlatform.Windows)} + {nameof(TargetPlatform.Linux)} + {nameof(TargetPlatform.MacOs)}.");
            Console.WriteLine($"- {nameof(TargetPlatform.Mobile)} - {nameof(TargetPlatform.Android)} + {nameof(TargetPlatform.AppleIOs)}.");
            Console.WriteLine(
                $"- {nameof(TargetPlatform.Consoles)} - {nameof(TargetPlatform.Switch)} + {nameof(TargetPlatform.Xbox)} + {nameof(TargetPlatform.Playstation4)}.");
            Console.WriteLine($"- {nameof(TargetPlatform.All)} - All platforms (default).");

            Console.WriteLine("\nPlatforms flagged as 'unsupported' can be built but Decay Engine does not support them yet.");

            return 0;
        }
    }
}