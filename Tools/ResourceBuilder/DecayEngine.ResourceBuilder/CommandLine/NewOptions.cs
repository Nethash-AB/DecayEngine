using CommandLine;

namespace DecayEngine.ResourceBuilder.CommandLine
{
    [Verb("new", HelpText = "Creates a new game project.")]
    public class NewOptions
    {
        [Option('d', "directory", Required = false, Default = "./", HelpText = "Path to directory the new project will be created at.")]
        public string Path { get; set; }

        [Option('f', "force", Default = false, HelpText = "Overwrite existing files in the provided directory.")]
        public bool Clean { get; set; }
    }
}