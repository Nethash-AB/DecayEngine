using DecayEngine.DecPakLib.Resource.RootElement.Sound;
using DecayEngine.ModuleSDK.Capability;
using DecayEngine.ModuleSDK.Component.SoundBank;
using DecayEngine.ModuleSDK.Engine.Sound;

namespace DecayEngine.ModuleSDK.Component.Sound
{
    public interface ISound : IComponent<SoundResource>, IAudioEmitter
    {
        bool AutoPlayOnActive { get; set; }
        ISoundBank SoundBank { get; }
        float Volume { get; set; }
        float Pitch { get; set; }
        int TimelinePosition { get; set; }

        event OnSoundStop OnSoundStop;
        event OnBecameAudible OnBecameAudible;
        event OnBecameNotAudible OnBecameNotAudible;
        event OnTimelineMarkerReached OnTimelineMarkerReached;
        event OnTimelineBeat OnTimelineBeat;

        float GetParameter(string name);
        void SetParameter(string name, float value);
        void TriggerCue();
    }
}