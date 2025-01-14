using DecayEngine.ModuleSDK.Exports.Attributes;

namespace DecayEngine.ModuleSDK.Exports.BaseExports.Sound
{
    [ScriptExportEnum("SoundPlaybackStatus", "Represents the playback status of a `Sound`.")]
    public enum SoundPlaybackStatus
    {
        [ScriptExportField("The playback status cannot be retrieved or querying this property is not supported by the current sound engine.")]
        Error = -1,
        [ScriptExportField("Playing.")]
        Playing = 0,
        [ScriptExportField("Sustaining. The meaning of this status depends on the sound engine being used.")]
        Sustaining = 1,
        [ScriptExportField("Stopped.")]
        Stopped = 2,
        [ScriptExportField("Starting. The meaning of this status depends on the sound engine being used.")]
        Starting = 3,
        [ScriptExportField("Stopping. The meaning of this status depends on the sound engine being used.")]
        Stopping = 4
    }
}