using System;
using System.Runtime.InteropServices;
using DecayEngine.DecPakLib;
using DecayEngine.DecPakLib.Resource.RootElement.Sound;
using DecayEngine.Fmod.Managed.AudioMath;
using DecayEngine.Fmod.Managed.FmodInterop;
using DecayEngine.Fmod.Managed.FmodInterop.Studio;
using DecayEngine.ModuleSDK;
using DecayEngine.ModuleSDK.Capability;
using DecayEngine.ModuleSDK.Component.Sound;
using DecayEngine.ModuleSDK.Component.SoundBank;
using DecayEngine.ModuleSDK.Engine.Sound;
using DecayEngine.ModuleSDK.Exports.BaseExports.Sound;
using DecayEngine.ModuleSDK.Object;
using DecayEngine.ModuleSDK.Object.GameObject;
using DecayEngine.ModuleSDK.Object.Transform;

namespace DecayEngine.Fmod.Managed.Component.Sound
{
    public class SoundComponent : ISound
    {
        private IGameObject _parent;

        public bool Destroyed { get; private set; }
        public string Name { get; set; }

        public IGameObject Parent => _parent;
        public ByReference<IGameObject> ParentByRef => () => ref _parent;

        public Type ExportType => typeof(SoundExport);
        public SoundResource Resource { get; internal set; }

        public bool Active
        {
            get
            {
                if (Parent == null)
                {
                    return false;
                }

                return _soundHandle.hasHandle() && _soundHandle.isValid() && _soundInstance.hasHandle() && _soundInstance.isValid();
            }
            set
            {
                if (!Active && value)
                {
                    Load();
                    if (AutoPlayOnActive)
                    {
                        Play();
                    }
                }
                else if (Active && !value)
                {
                    Unload();
                }
            }
        }

        public Transform Transform => _transform;
        public ByReference<Transform> TransformByRef => () => ref _transform;
        public Transform WorldSpaceTransform => this.GetWorldSpaceTransform();

        public bool AutoPlayOnActive { get; set; }

        public int PlaybackStatus => GetPlaybackState();

        public ISoundBank SoundBank { get; }

        public float Volume
        {
            get
            {
                if (!Active) return float.NegativeInfinity;

                RESULT result = _soundInstance.getVolume(out float volume);
                return result != RESULT.OK ? float.NegativeInfinity : volume;
            }
            set
            {
                if (!Active) return;

                _soundInstance.setVolume(value);
            }
        }

        public float Pitch
        {
            get
            {
                if (!Active) return float.NegativeInfinity;

                RESULT result = _soundInstance.getPitch(out float pitch);
                return result != RESULT.OK ? float.NegativeInfinity : pitch;
            }
            set
            {
                if (!Active) return;

                _soundInstance.setPitch(value);
            }
        }

        public int TimelinePosition
        {
            get
            {
                if (!Active) return int.MinValue;

                RESULT result = _soundInstance.getTimelinePosition(out int position);
                return result != RESULT.OK ? int.MinValue : position;
            }
            set
            {
                if (!Active) return;

                _soundInstance.setTimelinePosition(value);
            }
        }

        public event OnSoundStop OnSoundStop;
        public event OnBecameAudible OnBecameAudible;
        public event OnBecameNotAudible OnBecameNotAudible;
        public event OnTimelineMarkerReached OnTimelineMarkerReached;
        public event OnTimelineBeat OnTimelineBeat;

        private Transform _transform;
        private EventDescription _soundHandle;
        private EventInstance _soundInstance;
        private readonly EVENT_CALLBACK _eventCallbackDelegate;

        public SoundComponent(ISoundBank bank, EventDescription soundHandle)
        {
            _transform = new Transform();

            SoundBank = bank;
            _soundHandle = soundHandle;

            _eventCallbackDelegate = EventCallback;
        }

        public void SetParent(IGameObject parent)
        {
            _parent?.RemoveComponent(this);

            parent?.AttachComponent(this);
            _parent = parent;
        }

        public IParentable<IGameObject> AsParentable<T>() where T : IGameObject
        {
            return this;
        }

        ~SoundComponent()
        {
            Destroy();
        }

        public void UpdatePositionalAudio()
        {
            _soundHandle.is3D(out bool is3D);
            if (!is3D) return;

            _soundInstance.set3DAttributes(WorldSpaceTransform.Get3DAttributes());
        }

        public void Play()
        {
            if (!Active) return;

            GameEngine.SoundEngine.EngineThread.ExecuteOnThread(() =>
            {
                if (PlaybackStatus != (int) PLAYBACK_STATE.STOPPED) return;

                UpdatePositionalAudio();
                _soundInstance.start();
            });
        }

        public void Pause()
        {
            if (!Active) return;

            GameEngine.SoundEngine.EngineThread.ExecuteOnThread(() =>
            {
                if (_soundInstance.getPaused(out bool paused) != RESULT.OK || paused) return;
                _soundInstance.setPaused(true);
            });
        }

        public void UnPause()
        {
            if (!Active) return;

            GameEngine.SoundEngine.EngineThread.ExecuteOnThread(() =>
            {
                if (_soundInstance.getPaused(out bool paused) != RESULT.OK || !paused) return;
                _soundInstance.setPaused(false);
            });
        }

        public void Stop(bool fadeOut)
        {
            if (!Active) return;

            GameEngine.SoundEngine.EngineThread.ExecuteOnThread(() =>
            {
                int playbackStatus = PlaybackStatus;
                if (
                    playbackStatus == (int) PLAYBACK_STATE.PLAYING ||
                    playbackStatus == (int) PLAYBACK_STATE.STARTING ||
                    playbackStatus == (int) PLAYBACK_STATE.SUSTAINING
                )
                {
                    _soundInstance.stop(fadeOut ? STOP_MODE.ALLOWFADEOUT : STOP_MODE.IMMEDIATE);
                }
            });
        }

        public float GetParameter(string name)
        {
            if (!Active) return float.NegativeInfinity;

            RESULT result = _soundInstance.getParameterByName(name, out float value);
            return result != RESULT.OK ? float.NegativeInfinity : value;
        }

        public void SetParameter(string name, float value)
        {
            if (!Active) return;

            _soundInstance.setParameterByName(name, value);
        }

        public void TriggerCue()
        {
            if (!Active) return;

            _soundInstance.triggerCue();
        }

        private int GetPlaybackState()
        {
            if (!Active) return -1;

            RESULT result = _soundInstance.getPlaybackState(out PLAYBACK_STATE state);
            if (result != RESULT.OK) return -1;

            return (int) state;
        }

        private RESULT EventCallback(EVENT_CALLBACK_TYPE type, EventInstance _event, IntPtr parameters)
        {
            switch (type)
            {
                case EVENT_CALLBACK_TYPE.STOPPED:
                {
                    OnSoundStop?.Invoke(this);
                    return RESULT.OK;
                }
                case EVENT_CALLBACK_TYPE.VIRTUAL_TO_REAL:
                {
                    OnBecameAudible?.Invoke(this);
                    return RESULT.OK;
                }
                case EVENT_CALLBACK_TYPE.REAL_TO_VIRTUAL:
                {
                    OnBecameNotAudible?.Invoke(this);
                    return RESULT.OK;
                }
                case EVENT_CALLBACK_TYPE.TIMELINE_MARKER:
                {
                    TIMELINE_MARKER_PROPERTIES timelineMarkerProperties = Marshal.PtrToStructure<TIMELINE_MARKER_PROPERTIES>(parameters);
                    OnTimelineMarkerReached?.Invoke(this, timelineMarkerProperties.name, timelineMarkerProperties.position);
                    return RESULT.OK;
                }
                case EVENT_CALLBACK_TYPE.TIMELINE_BEAT:
                {
                    TIMELINE_BEAT_PROPERTIES timelineBeatProperties = Marshal.PtrToStructure<TIMELINE_BEAT_PROPERTIES>(parameters);
                    OnTimelineBeat?.Invoke(this,
                        timelineBeatProperties.bar, timelineBeatProperties.beat, timelineBeatProperties.position, timelineBeatProperties.tempo);
                    return RESULT.OK;
                }
                default:
                    return RESULT.OK;
            }
        }

        private void Load()
        {
            GameEngine.SoundEngine.EngineThread.ExecuteOnThread(() =>
            {
                if (!SoundBank.Active)
                {
                    SoundBank.Active = true;
                }

                _soundHandle.loadSampleData();
                _soundHandle.createInstance(out _soundInstance);

                _soundInstance.setCallback(_eventCallbackDelegate);

                return _soundInstance;
            });
        }

        private void Unload()
        {
            GameEngine.SoundEngine.EngineThread.ExecuteOnThread(() =>
            {
                _soundInstance.release();
                _soundInstance.clearHandle();
                _soundHandle.unloadSampleData();
            });
        }

        public void Destroy()
        {
            GameEngine.SoundEngine.UntrackSound(this);
            SetParent(null);

            _soundHandle.clearHandle();

            _transform = null;

            Destroyed = true;
        }
    }
}