using DecayEngine.ModuleSDK.Exports.Attributes;
// ReSharper disable InconsistentNaming

namespace DecayEngine.ModuleSDK.Logging.Exports
{
    [ScriptExportNamespace("console", "Contains aliases for the Logging namespace.", true)]
    public static class ConsoleNamespaceExport
    {
        private delegate void LogDelegate(string message);
        [ScriptExportMethod("Appends a line to the log with `LogSeverity.Info` severity.", typeof(ConsoleNamespaceExport), typeof(LogDelegate))]
        public static void log(
            [ScriptExportParameter("The message to log")] string message
        )
        {
            LoggingNamespaceExport.Append(LogSeverity.Info, "ConsoleLog", message);
        }
    }
}