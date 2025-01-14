namespace DecayEngine.ModuleSDK.Exports.BaseExports.Sound
{
    public delegate void OnSoundStopExport(SoundExport sound);
    public delegate void OnBecameAudibleExport(SoundExport sound);
    public delegate void OnBecameNotAudibleExport(SoundExport sound);
    public delegate void OnTimelineMarkerReachedExport(SoundExport sound, string name, int position);
    public delegate void OnTimelineBeatExport(SoundExport sound, int bar, int beat, int position, float tempo);
}