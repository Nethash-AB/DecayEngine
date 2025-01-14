using System;
using DecayEngine.ModuleSDK.Exports.Attributes;

namespace DecayEngine.ModuleSDK.Exports.BaseExports.SoundController
{
    [ScriptExportNamespace("SoundController", "Provides functionality to handle sound.")]
    public static class SoundControllerExport
    {
        [ScriptExportProperty("Whether the audio system is globally muted.", (Type) null, typeof(SoundControllerExport))]
        public static bool AudioMuted => GameEngine.SoundEngine.AudioMuted;
        [ScriptExportProperty("The current global volume of the audio system.", (Type) null, typeof(SoundControllerExport))]
        public static float GlobalVolume => GameEngine.SoundEngine.GlobalVolume;

        private delegate void MuteAudioDelegate();
        [ScriptExportMethod("Globally mutes the audio system.", typeof(SoundControllerExport),
            typeof(MuteAudioDelegate))]
        public static void MuteAudio() => GameEngine.SoundEngine.MuteAudio();

        private delegate void UnMuteAudioDelegate();
        [ScriptExportMethod("Globally unmutes the audio system.", typeof(SoundControllerExport),
            typeof(UnMuteAudioDelegate))]
        public static void UnmuteAudio() => GameEngine.SoundEngine.UnmuteAudio();

        private delegate void SetGlobalVolumeDelegate(float volume);
        [ScriptExportMethod("Sets the global volume the audio system.", typeof(SoundControllerExport),
            typeof(SetGlobalVolumeDelegate))]
        public static void SetGlobalVolume(
            [ScriptExportParameter("The value to set the global volume to.")] float volume
        ) => GameEngine.SoundEngine.SetGlobalVolume(volume);
    }
}