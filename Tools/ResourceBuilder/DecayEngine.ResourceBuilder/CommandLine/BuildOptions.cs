using System.Collections.Generic;
using CommandLine;
using DecayEngine.DecPakLib.Platform;

namespace DecayEngine.ResourceBuilder.CommandLine
{
    [Verb("build", HelpText = "Builds resources into decpak and decmeta files.")]
    public class BuildOptions
    {
        [Option('i', "input", Required = true, HelpText = "Path to directory containing resource files.")]
        public string InputPath { get; set; }

        [Option('o', "output", Required = true, HelpText = "Path to directory where decpak and decmeta files will be placed after compilation.")]
        public string OutputPath { get; set; }

        [Option('p', "platforms", Required = false, Default = null, Separator = ',', Min = 1,
            HelpText = "List of comma delimited platforms to build for. " +
                       "For multiplatform targets use platform aliases, each individually provided platform will be built separately. " +
                       "Please note that while all platforms can be built, " +
                       "Decay Engine might not support all platforms at this time. For a list of platforms invoke this tool with the 'list-platforms' verb.")]
        public IEnumerable<TargetPlatform> TargetPlatforms { get; set; }
    }
}