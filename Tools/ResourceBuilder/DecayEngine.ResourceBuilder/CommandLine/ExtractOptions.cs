using CommandLine;

namespace DecayEngine.ResourceBuilder.CommandLine
{
    [Verb("extract", HelpText = "Extracts decpak and decmeta files back into its original resources.")]
    public class ExtractOptions
    {
        [Option('i', "input", Required = true, HelpText = "Path to the decmeta file to extract.")]
        public string InputPath { get; set; }

        [Option('o', "output", Required = true, HelpText = "Path to directory where resource files will be placed after extraction.")]
        public string OutputPath { get; set; }
    }
}