using CommandLine;

namespace DecayEngine.TypingsGenerator
{
    public class CommandLineOptions
    {
        [Option('o', "output", Required = true, HelpText = "Path to directory where the typings will be generated.")]
        public string OutputPath { get; set; }
    }
}