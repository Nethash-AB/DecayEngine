using DecayEngine.ModuleSDK.Exports.Attributes;

namespace DecayEngine.ModuleSDK.Logging.Exports
{
    [ScriptExportNamespace("Logging", "Contains wrappers for Logging utilities.")]
    public static class LoggingNamespaceExport
    {
        private delegate void AppendDelegate(LogSeverity severity, string nameSpace, string message);
        [ScriptExportMethod("Appends a line to the log.", typeof(LoggingNamespaceExport), typeof(AppendDelegate))]
        public static void Append(
            [ScriptExportParameter("The severity of the message")] LogSeverity severity,
            [ScriptExportParameter("The namespace of the message. Adding too many namespaces makes the log hard to read, use with care.")] string nameSpace,
            [ScriptExportParameter("The message to log")] string message
        )
        {
            GameEngine.LogAppendLine(severity, nameSpace, message, 0, "ScriptEnvironment", "ScriptEnvironment");
        }
    }
}