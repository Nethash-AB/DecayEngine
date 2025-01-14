using System.IO;
using System.Threading.Tasks;
using DecayEngine.ModuleSDK.Logging;
using Android.Util;

namespace DecayEngine.Android
{
    public class AndroidLogcatLogger : ILogger<AndroidLogcatLoggerOptions>
    {
        public LogSeverity MinimumSeverity { get; set; }

        public Task Init(AndroidLogcatLoggerOptions options)
        {
            MinimumSeverity = options.MinimumSeverity;

            return Task.CompletedTask;
        }

        public void AppendLine(LogSeverity severity, string nameSpace, string message, int lineNumber, string callerName, string sourcePath)
        {
            string debugOutput = $"{Path.GetFileNameWithoutExtension(sourcePath)}:{lineNumber}->{callerName}";
            string debugText = $"[{severity.ToString()}]({nameSpace}): {message} | {debugOutput}";

            switch (severity)
            {
                case LogSeverity.Debug:
                    Log.Debug(AndroidConstants.LogcatTag, debugText);
                    break;
                case LogSeverity.Error:
                    Log.Error(AndroidConstants.LogcatTag, debugText);
                    break;
                case LogSeverity.Info:
                    Log.Info(AndroidConstants.LogcatTag, debugText);
                    break;
                case LogSeverity.Warning:
                    Log.Warn(AndroidConstants.LogcatTag, debugText);
                    break;
                case LogSeverity.CriticalError:
                    Log.Error(AndroidConstants.LogcatTag, debugText);
                    break;
            }
        }
    }
}