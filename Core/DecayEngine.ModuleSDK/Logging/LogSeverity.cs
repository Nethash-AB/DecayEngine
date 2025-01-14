using DecayEngine.ModuleSDK.Exports.Attributes;
using DecayEngine.ModuleSDK.Logging.Exports;

namespace DecayEngine.ModuleSDK.Logging
{
    [ScriptExportEnum("LogSeverity", "Represents the different severity levels a log message can have.", typeof(LoggingNamespaceExport))]
    public enum LogSeverity
    {
        [ScriptExportField("Used for messages the user should never see unless an issue is being reported.")]
        Debug = 0,
        [ScriptExportField("Used for messages that inform about the flow of the engine or game but are not critical.")]
        Info = 1,
        [ScriptExportField("Used for messages that are worth looking into but are not going to cause trouble in most cases.")]
        Warning = 2,
        [ScriptExportField("Used for messages that represent a recoverable error.")]
        Error = 3,
        [ScriptExportField("Used for messages that represent an unrecoverable error. The engine should shutdown after emitting this kind of error.")]
        CriticalError = 4
    }
}