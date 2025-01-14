using DecayEngine.ModuleSDK.Logging;

namespace DecayEngine.Android
{
    public class AndroidLogcatLoggerOptions : ILoggerOptions
    {
        public LogSeverity MinimumSeverity { get; set; }
    }
}