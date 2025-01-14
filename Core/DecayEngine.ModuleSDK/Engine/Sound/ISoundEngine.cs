using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DecayEngine.ModuleSDK.Capability;
using DecayEngine.ModuleSDK.Component.SoundBank;
using DecayEngine.ModuleSDK.Exports.BaseExports.Sound;

namespace DecayEngine.ModuleSDK.Engine.Sound
{
    public interface ISoundEngine : IMultiThreadedEngine
    {
        IEnumerable<IAudioListener> AudioListeners { get; }
        IEnumerable<IAudioEmitter> AudioEmitters { get; }
        IEnumerable<ISoundBank> SoundBanks { get; }

        bool AudioMuted { get; }
        float GlobalVolume { get; }

        Task<object> LoadBank(byte[] bankData);
        object LoadEvent(string path);

        void MuteAudio();
        void UnmuteAudio();
        void SetGlobalVolume(float volume);

        int MarshalPlaybackStatus(SoundPlaybackStatus status);
        SoundPlaybackStatus MarshalPlaybackStatus(int status);
        void AddListener(IAudioListener listener);
        void RemoveListener(IAudioListener listener);
        void TrackSound(IAudioEmitter sound);
        void UntrackSound(IAudioEmitter sound);
        void TrackSoundBank(ISoundBank soundBank);
        void UntrackSoundBank(ISoundBank soundBank);
    }

    public interface ISoundEngine<in TOptions> : ISoundEngine, IMultiThreadedEngine<TOptions> where TOptions : IEngineOptions
    {
    }
}