using CommandLine;

namespace DecayEngine.Standalone
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class CommandLineOptions
    {
        [Option('b', "bundlePath", Default = "./Data/Resources.decmeta", Required = false, HelpText = "Path to decmeta file to load.")]
        public string BundlePath { get; set; }
        [Option('s', "initSceneId", Default = "test_scene", Required = false, HelpText = "Id of the initialization scene.")]
        public string InitSceneId { get; set; }
    }
}