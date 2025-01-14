using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DecayEngine.ModuleSDK.Capability;
using DecayEngine.ModuleSDK.Component;
using DecayEngine.ModuleSDK.Component.SoundBank;
using DecayEngine.ModuleSDK.Engine.Script;
using DecayEngine.ModuleSDK.Engine.Sound;
using DecayEngine.ModuleSDK.Exports.BaseExports.Sound;
using DecayEngine.ModuleSDK.Threading;

namespace DecayEngine.StubEngines.Sound
{
    public class StubSoundEngine : ISoundEngine<StubEngineOptions>
    {
        private readonly List<IAudioListener> _audioListeners;
        private readonly List<IAudioEmitter> _audioEmitters;
        private readonly List<ISoundBank> _soundBanks;

        public IEngineThread EngineThread { get; private set; }

        public List<IComponentFactory> ComponentFactories { get; }
        public ScriptExports ScriptExports { get; }

        public IEnumerable<IAudioListener> AudioListeners => _audioListeners;
        public IEnumerable<IAudioEmitter> AudioEmitters => _audioEmitters;
        public IEnumerable<ISoundBank> SoundBanks => _soundBanks;

        public bool AudioMuted { get; private set; }
        public float GlobalVolume { get; private set; }

        public StubSoundEngine()
        {
            ComponentFactories = new List<IComponentFactory>();
            ScriptExports = new ScriptExports();
            _audioListeners = new List<IAudioListener>();
            _audioEmitters = new List<IAudioEmitter>();
            _soundBanks = new List<ISoundBank>();
        }

        public Task Init(StubEngineOptions options)
        {
            EngineThread = new ManagedEngineThread("", 128);
            EngineThread.Run();

            return Task.CompletedTask;
        }

        public Task Shutdown()
        {
            EngineThread.Stop();
            return Task.CompletedTask;
        }

        public Task<object> LoadBank(byte[] bankData)
        {
            return new Task<object>(() => null);
        }

        public object LoadEvent(string path)
        {
            return null;
        }

        public void MuteAudio()
        {
            AudioMuted = true;
        }

        public void UnmuteAudio()
        {
            AudioMuted = false;
        }

        public void SetGlobalVolume(float volume)
        {
            GlobalVolume = volume;
        }

        public int MarshalPlaybackStatus(SoundPlaybackStatus status)
        {
            return (int) SoundPlaybackStatus.Error;
        }

        public SoundPlaybackStatus MarshalPlaybackStatus(int status)
        {
            return SoundPlaybackStatus.Error;
        }

        public void AddListener(IAudioListener listener)
        {
            if (_audioListeners.Contains(listener)) return;

            _audioListeners.Add(listener);
        }

        public void RemoveListener(IAudioListener listener)
        {
            if (!_audioListeners.Contains(listener)) return;

            _audioListeners.Remove(listener);
        }

        public void TrackSound(IAudioEmitter sound)
        {
            if (_audioEmitters.Contains(sound)) return;

            _audioEmitters.Add(sound);
        }

        public void UntrackSound(IAudioEmitter sound)
        {
            if (!_audioEmitters.Contains(sound)) return;

            _audioEmitters.Remove(sound);
        }

        public void TrackSoundBank(ISoundBank soundBank)
        {
            if (_soundBanks.Contains(soundBank)) return;

            _soundBanks.Add(soundBank);
        }

        public void UntrackSoundBank(ISoundBank soundBank)
        {
            if (!_soundBanks.Contains(soundBank)) return;

            _soundBanks.Remove(soundBank);
        }
    }
}