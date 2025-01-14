namespace DecayEngine.ModuleSDK.Capability
{
    public interface IAudioEmitter : ITransformable
    {
        int PlaybackStatus { get; }

        void UpdatePositionalAudio();
        void Play();
        void Pause();
        void UnPause();
        void Stop(bool fadeOut);
    }
}