using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DecayEngine.DecPakLib.Resource.RootElement.SoundBank;
using DecayEngine.Fmod.Managed.AudioMath;
using DecayEngine.Fmod.Managed.Component.Sound;
using DecayEngine.Fmod.Managed.Component.SoundBank;
using DecayEngine.Fmod.Managed.FmodInterop;
using DecayEngine.Fmod.Managed.FmodInterop.Studio;
using DecayEngine.ModuleSDK;
using DecayEngine.ModuleSDK.Capability;
using DecayEngine.ModuleSDK.Component;
using DecayEngine.ModuleSDK.Component.SoundBank;
using DecayEngine.ModuleSDK.Engine.Script;
using DecayEngine.ModuleSDK.Engine.Sound;
using DecayEngine.ModuleSDK.Exports.BaseExports.Sound;
using DecayEngine.ModuleSDK.Exports.BaseExports.SoundController;
using DecayEngine.ModuleSDK.Logging;
using DecayEngine.ModuleSDK.Threading;
using ADVANCEDSETTINGS = DecayEngine.Fmod.Managed.FmodInterop.ADVANCEDSETTINGS;
using Debug = DecayEngine.Fmod.Managed.FmodInterop.Debug;
using INITFLAGS = DecayEngine.Fmod.Managed.FmodInterop.Studio.INITFLAGS;

namespace DecayEngine.Fmod.Managed
{
    public class FmodEngine : ISoundEngine<FmodEngineOptions>
    {
        private static DEBUG_CALLBACK _fmodDebugCallback = FmodEngine.OnFmodDebugMessage;

        private FmodEngineOptions _options;

        private RESULT _initResult;
        private ADVANCEDSETTINGS _advancedsettings;
        private INITFLAGS _studioInitFlags;
        private OUTPUTTYPE _outputType;
        private FmodInterop.Studio.System _studioSystem;
        private FmodInterop.System _coreSystem;
        private Bus _masterBus;

        private readonly List<IAudioListener> _audioListeners;
        private readonly List<IAudioEmitter> _audioEmitters;
        private readonly List<ISoundBank> _soundBanks;

        private bool _initialized;

        public IEngineThread EngineThread { get; private set; }

        public List<IComponentFactory> ComponentFactories { get; }
        public ScriptExports ScriptExports { get; }

        public IEnumerable<IAudioListener> AudioListeners => _audioListeners;
        public IEnumerable<IAudioEmitter> AudioEmitters => _audioEmitters;
        public IEnumerable<ISoundBank> SoundBanks => _soundBanks;

        public bool AudioMuted => EngineThread.ExecuteOnThread(() =>
        {
            if (!_initialized) return true;

            RESULT result = _masterBus.getMute(out bool mute);
            CheckResult(result, "Studio::System::Bus::getMute");

            return mute;
        });

        public float GlobalVolume => EngineThread.ExecuteOnThread(() =>
        {
            if (!_initialized) return 0f;

            RESULT result = _masterBus.getVolume(out float volume);
            CheckResult(result, "Studio::System::Bus::getVolume");

            return volume;
        });

        public FmodEngine()
        {
            ComponentFactories = new List<IComponentFactory>
            {
                new SoundFactory(),
                new SoundBankFactory()
            };

            ScriptExports = new ScriptExports();
            ScriptExports.Types.Add(typeof(SoundControllerExport));

            _audioListeners = new List<IAudioListener>();
            _audioEmitters = new List<IAudioEmitter>();
            _soundBanks = new List<ISoundBank>();
        }

        public Task Init(FmodEngineOptions options)
        {
            _options = options;

            EngineThread = new ManagedEngineThread("Fmod", 128);
            Task initTask = EngineThread.ExecuteOnThreadAsync(InitEngine);

            EngineThread.Run();
            return initTask;
        }

        public Task Shutdown()
        {
            EngineThread.Stop();

            if (_studioSystem.isValid())
            {
                _studioSystem.release();
                _studioSystem.clearHandle();
            }

            GameEngine.LogAppendLine(LogSeverity.Info, "Fmod", "Fmod terminated.");
            return Task.CompletedTask;
        }

        public Task<object> LoadBank(byte[] bankData)
        {
            return EngineThread.ExecuteOnThreadAsync(() =>
            {
                _studioSystem.loadBankMemory(bankData, LOAD_BANK_FLAGS.NORMAL, out Bank bank);
                return (object) bank;
            });
        }

        public object LoadEvent(string path)
        {
            return EngineThread.ExecuteOnThread<object>(() =>
            {
                Guid eventGuid = GetEventGuid(path);
                if (eventGuid == Guid.Empty) return null;

                RESULT result = _studioSystem.getEventByID(eventGuid, out EventDescription soundHandle);
                if (result != RESULT.OK)
                {
                    return null;
                }

                if (!soundHandle.isValid()) return null;

                return soundHandle;
            });
        }

        public void MuteAudio()
        {
            EngineThread.ExecuteOnThread(() =>
            {
                if (!_initialized || AudioMuted) return;

                RESULT result = _masterBus.setMute(true);
                CheckResult(result, "Studio::System::Bus::setMute");
            });
        }

        public void UnmuteAudio()
        {
            EngineThread.ExecuteOnThread(() =>
            {
                if (!_initialized || !AudioMuted) return;

                RESULT result = _masterBus.setMute(false);
                CheckResult(result, "Studio::System::Bus::setMute");
            });
        }

        public void SetGlobalVolume(float volume)
        {
            EngineThread.ExecuteOnThread(() =>
            {
                if (!_initialized) return;

                RESULT result = _masterBus.setVolume(volume);
                CheckResult(result, "Studio::System::Bus::setVolume");
            });
        }

        public int MarshalPlaybackStatus(SoundPlaybackStatus status)
        {
            return status switch
            {
                SoundPlaybackStatus.Playing => (int) PLAYBACK_STATE.PLAYING,
                SoundPlaybackStatus.Sustaining => (int) PLAYBACK_STATE.SUSTAINING,
                SoundPlaybackStatus.Stopped => (int) PLAYBACK_STATE.STOPPED,
                SoundPlaybackStatus.Starting => (int) PLAYBACK_STATE.STARTING,
                SoundPlaybackStatus.Stopping => (int) PLAYBACK_STATE.STOPPING,
                _ => -1
            };
        }

        public SoundPlaybackStatus MarshalPlaybackStatus(int status)
        {
            return (PLAYBACK_STATE) status switch
            {
                PLAYBACK_STATE.PLAYING => SoundPlaybackStatus.Playing,
                PLAYBACK_STATE.SUSTAINING => SoundPlaybackStatus.Sustaining,
                PLAYBACK_STATE.STOPPED => SoundPlaybackStatus.Stopped,
                PLAYBACK_STATE.STARTING => SoundPlaybackStatus.Starting,
                PLAYBACK_STATE.STOPPING => SoundPlaybackStatus.Stopping,
                _ => SoundPlaybackStatus.Error
            };
        }

        public void AddListener(IAudioListener listener)
        {
            if (_audioListeners.Contains(listener)) return;

            if (_audioListeners.Count + 1 >= CONSTANTS.MAX_LISTENERS)
            {
                GameEngine.LogAppendLine(
                    LogSeverity.Warning,
                    "Fmod",
                    $"Max number of listeners reached, new listener will be ignored. Max listeners allowed: {CONSTANTS.MAX_LISTENERS}"
                );
                return;
            }

            _audioListeners.Add(listener);

            EngineThread.ExecuteOnThread(() =>
            {
                RESULT result = _studioSystem.setNumListeners(_audioListeners.Count);
                CheckResult(result, "Studio::System::setNumListeners");
            });
        }

        public void RemoveListener(IAudioListener listener)
        {
            if (!_audioListeners.Contains(listener)) return;

            _audioListeners.Remove(listener);

            EngineThread.ExecuteOnThread(() =>
            {
                RESULT result = _studioSystem.setNumListeners(_audioListeners.Count);
                CheckResult(result, "Studio::System::setNumListeners");
            });
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

        private Guid GetEventGuid(string path)
        {
            Guid guid;
            if (path.StartsWith("{"))
            {
                Util.parseID(path, out guid);
            }
            else
            {
                RESULT result = _studioSystem.lookupID(path, out guid);
                if (result == RESULT.ERR_EVENT_NOTFOUND)
                {
                    GameEngine.LogAppendLine(LogSeverity.Error, "Fmod", $"No sound event with ID ({path}) found in loaded sound banks.");
                    return Guid.Empty;
                }
            }
            return guid;
        }

        private void InitEngine()
        {
            _advancedsettings = new ADVANCEDSETTINGS
            {
                randomSeed = (uint) DateTime.Now.Ticks
            };

            _studioInitFlags = INITFLAGS.NORMAL | INITFLAGS.DEFERRED_CALLBACKS | INITFLAGS.LOAD_FROM_UPDATE | INITFLAGS.SYNCHRONOUS_UPDATE;
#if DEBUG
            _studioInitFlags |= INITFLAGS.LIVEUPDATE;
            _advancedsettings.profilePort = 9264;

            RESULT result = Debug.Initialize(DEBUG_FLAGS.LOG, DEBUG_MODE.CALLBACK, _fmodDebugCallback);
            if (result != RESULT.OK)
            {
                throw new Exception($"Error initializing Fmod Debug System: {result}");
            }
#endif

            _outputType = OUTPUTTYPE.AUTODETECT;
            _initResult = RESULT.OK;

            if (!InitSoundSystem())
            {
                GameEngine.LogAppendLine(LogSeverity.CriticalError, "Fmod", "Error initializing Fmod Sound System.");
                return;
            }

            GameEngine.OnScenePreload += (scene, isInit) =>
            {
                if (!isInit) return;

                GameEngine.LogAppendLine(LogSeverity.Info, "Fmod", "Loading master banks.");
                List<SoundBankResource> masterBanks = GameEngine.ResourceBundles
                    .SelectMany(bundle =>
                        bundle.Resources
                            .OfType<SoundBankResource>()
                            .Where(res => res.Type == SoundBankType.FmodMaster || res.Type == SoundBankType.FmodStrings))
                    .ToList();

                foreach (SoundBankResource stringBankResource in masterBanks.Where(res => res.Type == SoundBankType.FmodStrings))
                {
                    SoundBankComponent component = new SoundBankComponent
                    {
                        Resource = stringBankResource,
                        Name = stringBankResource.Id,
                        Persistent = true
                    };
                    scene.AttachComponent(component);
                    TrackSoundBank(component);
                    component.Active = true;
                }

                foreach (SoundBankResource masterBankResource in masterBanks.Where(res => res.Type == SoundBankType.FmodMaster))
                {
                    SoundBankComponent component = new SoundBankComponent
                    {
                        Resource = masterBankResource,
                        Name = masterBankResource.Id,
                        Persistent = true
                    };
                    scene.AttachComponent(component);
                    TrackSoundBank(component);
                    component.Active = true;
                }

                RESULT resBus = _studioSystem.getBus("bus:/", out _masterBus);
                CheckResult(resBus, "Studio::System::getBus");

                _initialized = true;
            };

            EngineThread.OnUpdate += Loop;

            GameEngine.LogAppendLine(LogSeverity.Info, "Fmod", $"Fmod loaded. Thread ID: {EngineThread.ThreadId}");
            if ((_studioInitFlags & INITFLAGS.LIVEUPDATE) != 0)
            {
                GameEngine.LogAppendLine(LogSeverity.Info, "Fmod", $"Fmod Live Update listening on port: {_advancedsettings.profilePort}.");
            }
        }

        private bool InitSoundSystem()
        {
            RESULT result = FmodInterop.Studio.System.create(out _studioSystem);
            CheckResult(result, "FmodInterop.Studio.System.create");

            result = _studioSystem.getCoreSystem(out _coreSystem);
            CheckResult(result, "FmodInterop.Studio.System.getCoreSystem");

            result = _coreSystem.setDSPBufferSize(2048, 2);
            CheckResult(result, "FmodInterop.System.setDSPBufferSize");

            result = _coreSystem.setSoftwareChannels(_options.VirtualChannelCount);
            CheckResult(result, "FmodInterop.System.setSoftwareChannels");

            result = _coreSystem.setOutput(_outputType);
            CheckResult(result, "FmodInterop.System.setOutput");

            result = _coreSystem.setSoftwareChannels(_options.RealChannelCount);
            CheckResult(result, "FmodInterop.System.setSoftwareChannels");

            result = _coreSystem.getDriverInfo(0, out Guid _,
                out int sampleRate, out SPEAKERMODE speakerMode, out int speakerModeChannels);
            CheckResult(result, "FmodInterop.System.getDriverInfo");

            result = _coreSystem.setSoftwareFormat(sampleRate, speakerMode, speakerModeChannels);
            CheckResult(result, "FmodInterop.System.setSoftwareFormat");

            result = _coreSystem.setAdvancedSettings(ref _advancedsettings);
            CheckResult(result, "FmodInterop.System.setAdvancedSettings");

            result = _studioSystem.initialize(_options.VirtualChannelCount, _studioInitFlags,
                FmodInterop.INITFLAGS.NORMAL | FmodInterop.INITFLAGS.MIX_FROM_UPDATE | FmodInterop.INITFLAGS.STREAM_FROM_UPDATE, IntPtr.Zero);
            if (result != RESULT.OK && _initResult == RESULT.OK)
            {
                _initResult = result;
                _outputType = OUTPUTTYPE.NOSOUND;
                GameEngine.LogAppendLine(LogSeverity.Error, "Fmod",
                    $"Error initializing Fmod Sound System ({result}), defaulting to no sound.");
                return InitSoundSystem();
            }
            CheckResult(result, "Studio::System::initialize");

            if (!_studioInitFlags.HasFlag(INITFLAGS.LIVEUPDATE)) return true;

            _studioSystem.flushCommands();

            result = _studioSystem.update();
            if (result != RESULT.ERR_NET_SOCKET_ERROR) return true;

            _studioInitFlags &= ~INITFLAGS.LIVEUPDATE;
            GameEngine.LogAppendLine(LogSeverity.Warning, "Fmod",
                "Error initializing Fmod Live Update, restarting with Live Update disabled.");
            result = _studioSystem.release();
            CheckResult(result, "FmodInterop.Studio.System.Release");

            return InitSoundSystem();
        }

        private void CheckResult(RESULT result, string cause)
        {
            if (result == RESULT.OK) return;

            if (_studioSystem.isValid())
            {
                _studioSystem.release();
                _studioSystem.clearHandle();
            }
            throw new Exception($"Fmod Sound System found an unexpected ({result}) error. Cause: {cause}.");
        }

        private static RESULT OnFmodDebugMessage(DEBUG_FLAGS flags, StringWrapper file, int line, StringWrapper func, StringWrapper message)
        {
            string msg = $"{((string) message).TrimEnd('\n')} | source:{(string) func}:{line}";
            if (flags.HasFlag(DEBUG_FLAGS.ERROR))
            {
                GameEngine.LogAppendLine(LogSeverity.Error, "FmodNative", msg);
            }
            else if (flags.HasFlag(DEBUG_FLAGS.WARNING))
            {
                GameEngine.LogAppendLine(LogSeverity.Warning, "FmodNative", msg);
            }
            else
            {
                GameEngine.LogAppendLine(LogSeverity.Debug, "FmodNative", msg);
            }

            return RESULT.OK;
        }

        private void Loop(double deltaTime)
        {
            foreach (IAudioEmitter sound in AudioEmitters)
            {
                sound.UpdatePositionalAudio();
            }

            int i = 0;
            foreach (IAudioListener listener in AudioListeners)
            {
                RESULT result = _studioSystem.setListenerAttributes(i, listener.WorldSpaceTransform.Get3DAttributes());
                CheckResult(result, "Studio::System::setListenerAttributes");
                i++;
            }

            if (_studioSystem.isValid())
            {
                RESULT result = _studioSystem.update();
                if (result != RESULT.OK)
                {
                    throw new Exception($"Error processing Fmod Sound System update. The result was: {result}");
                }
            }
        }
    }
}