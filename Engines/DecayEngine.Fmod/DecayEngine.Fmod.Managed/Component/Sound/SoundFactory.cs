using DecayEngine.DecPakLib.Resource.RootElement.Sound;
using DecayEngine.DecPakLib.Resource.RootElement.SoundBank;
using DecayEngine.Fmod.Managed.FmodInterop.Studio;
using DecayEngine.ModuleSDK;
using DecayEngine.ModuleSDK.Component;
using DecayEngine.ModuleSDK.Component.Sound;
using DecayEngine.ModuleSDK.Component.SoundBank;
using DecayEngine.ModuleSDK.Expression;

namespace DecayEngine.Fmod.Managed.Component.Sound
{
    public class SoundFactory : IComponentFactory<SoundComponent, SoundResource>, IComponentFactory<ISound, SoundResource>
    {
        ISound IComponentFactory<ISound, SoundResource>.CreateComponent(SoundResource resource)
        {
            return CreateComponent(resource);
        }

        public SoundComponent CreateComponent(SoundResource resource)
        {
            PropertyExpressionResolver resolver = new PropertyExpressionResolver(resource.Bank);
            SoundBankResource bankResource = resolver.Resolve<SoundBankResource>();

            ISoundBank bankComponent = (ISoundBank) GameEngine.CreateComponent(bankResource);
            if (bankComponent == null) return null;

            if (!bankComponent.Active)
            {
                bankComponent.Active = true;
            }

            object soundHandleRaw = GameEngine.SoundEngine.LoadEvent(resource.Event);
            if (soundHandleRaw == null) return null;

            EventDescription soundHandle = (EventDescription) GameEngine.SoundEngine.LoadEvent(resource.Event);

            SoundComponent component = new SoundComponent(bankComponent, soundHandle) {Resource = resource, Name = resource.Id};
            GameEngine.SoundEngine.TrackSound(component);

            return component;
        }
    }
}