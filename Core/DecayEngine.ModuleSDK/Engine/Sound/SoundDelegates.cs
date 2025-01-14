using DecayEngine.ModuleSDK.Component.Sound;

namespace DecayEngine.ModuleSDK.Engine.Sound
{
    public delegate void OnSoundStop(ISound sound);
    public delegate void OnBecameAudible(ISound sound);
    public delegate void OnBecameNotAudible(ISound sound);
    public delegate void OnTimelineMarkerReached(ISound sound, string name, int position);
    public delegate void OnTimelineBeat(ISound sound, int bar, int beat, int position, float tempo);
}