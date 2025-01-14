namespace DecayEngine.ModuleSDK.Logging.DebugConsole
{
    public class ConsoleLoggerOptions : ILoggerOptions
    {
        public LogSeverity MinimumSeverity { get; set; }
        public bool OutputPaths { get; set; }
        public bool UseDebugStream { get; set; }
    }
}