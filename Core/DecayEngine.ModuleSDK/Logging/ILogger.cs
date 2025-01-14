using System.Threading.Tasks;

namespace DecayEngine.ModuleSDK.Logging
{
    public interface ILogger
    {
        LogSeverity MinimumSeverity { get; set; }

        void AppendLine(LogSeverity severity, string nameSpace, string message, int lineNumber, string callerName, string sourcePath);
    }

    public interface ILogger<in TOptions> : ILogger where TOptions : ILoggerOptions
    {
        Task Init(TOptions options);
    }
}