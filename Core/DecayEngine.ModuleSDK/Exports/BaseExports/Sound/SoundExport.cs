using DecayEngine.DecPakLib;
using DecayEngine.ModuleSDK.Capability;
using DecayEngine.ModuleSDK.Component.Sound;
using DecayEngine.ModuleSDK.Exports.Attributes;
using DecayEngine.ModuleSDK.Exports.BaseExports.GameObject;
using DecayEngine.ModuleSDK.Exports.BaseExports.SoundBank;
using DecayEngine.ModuleSDK.Exports.BaseExports.Transform;
using DecayEngine.ModuleSDK.Exports.Capabilities;

namespace DecayEngine.ModuleSDK.Exports.BaseExports.Sound
{
    [ScriptExportClass("Sound", "Represents a Sound Component.")]
    public class SoundExport : ExportableManagedObject<ISound>, IComponentExport, ITransformableExport
    {
        public bool Active
        {
            get => Reference.Active;
            set => Reference.Active = value;
        }

        public bool ActiveInGraph => Reference.ActiveInGraph();

        public string Name
        {
            get => Reference.Name;
            set => Reference.Name = value;
        }

        public string Id => Reference.Resource.Id;

        public object Parent => Reference.Parent != null ? new GameObjectExport(Reference.Parent) : null;

        public override int Type => (int) ManagedExportType.Sound;

        [ScriptExportProperty("The transform of the `Sound`.")]
        public TransformExport Transform => new TransformExport(() => ref Reference.TransformByRef.Invoke());
        public TransformExport WorldSpaceTransform => new TransformExport(Reference.WorldSpaceTransform);

        [ScriptExportProperty("Defines whether the `Sound` will automatically play when it is activated.\n" +
                              "The behaviour of this property depends on the sound engine being used.")]
        public bool AutoPlayOnActive
        {
            get => Reference.AutoPlayOnActive;
            set => Reference.AutoPlayOnActive = value;
        }

        [ScriptExportProperty("The playback status of the `Sound`.\n" +
                              "The behaviour of this property depends on the sound engine being used.",
        typeOverride: typeof(SoundPlaybackStatus))]
        public int PlaybackStatus => (int) GameEngine.SoundEngine.MarshalPlaybackStatus(Reference.PlaybackStatus);

        [ScriptExportProperty("The `SoundBank` the `Sound` belongs to.")]
        public SoundBankExport SoundBank => new SoundBankExport(Reference.SoundBank);

        [ScriptExportProperty("The volume of the `Sound`.\n" +
                              "The behaviour of this property depends on the sound engine being used.")]
        public float Volume
        {
            get => Reference.Volume;
            set => Reference.Volume = value;
        }

        [ScriptExportProperty("The pitch of the `Sound`.\n" +
                              "The behaviour of this property depends on the sound engine being used.")]
        public float Pitch
        {
            get => Reference.Pitch;
            set => Reference.Pitch = value;
        }

        [ScriptExportProperty("The position on the timeline of the `Sound`.\n" +
                              "The behaviour of this property depends on the sound engine being used.")]
        public int TimelinePosition
        {
            get => Reference.TimelinePosition;
            set => Reference.TimelinePosition = value;
        }

        [ScriptExportProperty("An `EventHandler` that fires when the `Sound` stops playing.")]
        public EventExport<OnSoundStopExport> OnSoundStop { get; }

        [ScriptExportProperty("An `EventHandler` that fires when the `Sound` becomes audible due to 3D positional audio constraints.")]
        public EventExport<OnBecameAudibleExport> OnBecameAudible { get; }

        [ScriptExportProperty("An `EventHandler` that fires when the `Sound` becomes not audible due to 3D positional audio constraints.")]
        public EventExport<OnBecameNotAudibleExport> OnBecameNotAudible { get; }

        [ScriptExportProperty("An `EventHandler` that fires when the `Sound` reaches a named marker on the timeline.")]
        public EventExport<OnTimelineMarkerReachedExport> OnTimelineMarkerReached { get; }

        [ScriptExportProperty("An `EventHandler` that fires for each beat when the `Sound` reaches beat area on the timeline.")]
        public EventExport<OnTimelineBeatExport> OnTimelineBeat { get; }

        [ScriptExportMethod("Starts playing the `Sound`.")]
        public void Play()
        {
            Reference.Play();
        }

        [ScriptExportMethod("Pauses the `Sound`.")]
        public void Pause()
        {
            Reference.Pause();
        }

        [ScriptExportMethod("Unpauses the `Sound`.")]
        public void UnPause()
        {
            Reference.UnPause();
        }

        [ScriptExportMethod("Stops playing the `Sound`.")]
        public void Stop(
            [ScriptExportParameter("Whether to fade out the sound. `true` to allow for fade out to occur, `false` to stop playing immediately.")]
            bool fadeOut
        )
        {
            Reference.Stop(fadeOut);
        }

        [ScriptExportMethod("Gets a named property of the `Sound`.")]
        [return: ScriptExportReturn("The value of the specified property.\n" +
        "The behaviour of this function depends on the sound engine being used.")]
        public float GetParameter(
            [ScriptExportParameter("The name of the property to retrieve.")] string name
        )
        {
            return Reference.GetParameter(name);
        }

        [ScriptExportMethod("Sets the value of a named property of the `Sound`.\n" +
        "The behaviour of this function depends on the sound engine being used.")]
        public void SetParameter(
            [ScriptExportParameter("The name of the property to set the value of.")] string name,
            [ScriptExportParameter("The value to set the property to.")] float value
        )
        {
            Reference.SetParameter(name, value);
        }

        [ScriptExportMethod("Triggers the next cue found on the timeline of the `Sound`.\n" +
        "The behaviour of this function depends on the sound engine being used.")]
        public void TriggerCue()
        {
            Reference.TriggerCue();
        }

        [ExportCastConstructor]
        internal SoundExport(ByReference<ISound> referencePointer) : base(referencePointer)
        {
            OnSoundStop = new EventExport<OnSoundStopExport>();
            Reference.OnSoundStop += sound => OnSoundStop.Fire(new SoundExport(sound));

            OnBecameAudible = new EventExport<OnBecameAudibleExport>();
            Reference.OnBecameAudible += sound => OnBecameAudible.Fire(new SoundExport(sound));

            OnBecameNotAudible = new EventExport<OnBecameNotAudibleExport>();
            Reference.OnBecameNotAudible += sound => OnBecameNotAudible.Fire(new SoundExport(sound));

            OnTimelineMarkerReached = new EventExport<OnTimelineMarkerReachedExport>();
            Reference.OnTimelineMarkerReached += (sound, name, position) => OnTimelineMarkerReached.Fire(new SoundExport(sound), name, position);

            OnTimelineBeat = new EventExport<OnTimelineBeatExport>();
            Reference.OnTimelineBeat += (sound, bar, beat, position, tempo) =>
                OnTimelineBeat.Fire(new SoundExport(sound), bar, beat, position, tempo);
        }

        [ExportCastConstructor]
        internal SoundExport(ISound value) : base(value)
        {
            OnSoundStop = new EventExport<OnSoundStopExport>();
            Reference.OnSoundStop += sound => OnSoundStop.Fire(new SoundExport(sound));

            OnBecameAudible = new EventExport<OnBecameAudibleExport>();
            Reference.OnBecameAudible += sound => OnBecameAudible.Fire(new SoundExport(sound));

            OnBecameNotAudible = new EventExport<OnBecameNotAudibleExport>();
            Reference.OnBecameNotAudible += sound => OnBecameNotAudible.Fire(new SoundExport(sound));

            OnTimelineMarkerReached = new EventExport<OnTimelineMarkerReachedExport>();
            Reference.OnTimelineMarkerReached += (sound, name, position) => OnTimelineMarkerReached.Fire(new SoundExport(sound), name, position);

            OnTimelineBeat = new EventExport<OnTimelineBeatExport>();
            Reference.OnTimelineBeat += (sound, bar, beat, position, tempo) =>
                OnTimelineBeat.Fire(new SoundExport(sound), bar, beat, position, tempo);
        }
    }
}