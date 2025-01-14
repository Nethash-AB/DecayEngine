using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace DecayEngine.ModuleSDK.Logging.DebugConsole
{
    public class ConsoleLogger : ILogger<ConsoleLoggerOptions>
    {
        public LogSeverity MinimumSeverity { get; set; }
        public bool OutputPaths { get; set; }
        public bool UseDebugStream { get; set; }

        public Task Init(ConsoleLoggerOptions options)
        {
            MinimumSeverity = options.MinimumSeverity;
            OutputPaths = options.OutputPaths;
            UseDebugStream = options.UseDebugStream;

            return Task.CompletedTask;
        }

        public void AppendLine(LogSeverity severity, string nameSpace, string message, int lineNumber, string callerName, string sourcePath)
        {
            string debugOutput = OutputPaths ? $"{sourcePath}:{lineNumber}->{callerName}" : $"{Path.GetFileNameWithoutExtension(sourcePath)}:{lineNumber}->{callerName}";
            if (UseDebugStream)
            {
                Debug.WriteLine($"[{severity.ToString()}]({nameSpace}): {message} | {debugOutput}");
            }
            else
            {
                Console.WriteLine($"[{severity.ToString()}]({nameSpace}): {message} | {debugOutput}");
            }
        }
    }
}