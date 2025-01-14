using System.Linq;
using DecayEngine.DecPakLib.Resource.Expression.Property;
using DecayEngine.DecPakLib.Resource.RootElement.SoundBank;
using DecayEngine.ModuleSDK;
using DecayEngine.ModuleSDK.Component;
using DecayEngine.ModuleSDK.Component.SoundBank;
using DecayEngine.ModuleSDK.Expression;

namespace DecayEngine.Fmod.Managed.Component.SoundBank
{
    public class SoundBankFactory : IComponentFactory<SoundBankComponent, SoundBankResource>, IComponentFactory<ISoundBank, SoundBankResource>
    {
        ISoundBank IComponentFactory<ISoundBank, SoundBankResource>.CreateComponent(SoundBankResource resource)
        {
            return CreateComponent(resource);
        }

        public SoundBankComponent CreateComponent(SoundBankResource resource)
        {
            switch (resource.Type)
            {
                case SoundBankType.FmodMaster:
                case SoundBankType.FmodStrings:
                    return GameEngine.ActiveScene.Components.OfType<SoundBankComponent>()
                        .FirstOrDefault(bank => bank.Resource.Id == resource.Id);
                case SoundBankType.FmodSlave:
                {
                    foreach (SoundBankComponent bankComponent in GameEngine.ActiveScene.Components.OfType<SoundBankComponent>())
                    {
                        if (bankComponent.Resource == resource)
                        {
                            return bankComponent;
                        }
                    }

                    SoundBankComponent component = new SoundBankComponent {Resource = resource, Name = resource.Id};

                    if (resource.Requires != null)
                    {
                        foreach (IPropertyExpression requireExpression in resource.Requires)
                        {
                            PropertyExpressionResolver resolver = new PropertyExpressionResolver(requireExpression);
                            SoundBankResource requireRes = resolver.Resolve<SoundBankResource>();
                            component.RequiredBanks.Add(CreateComponent(requireRes));
                        }
                    }

                    GameEngine.ActiveScene.AttachComponent(component);

                    switch (component.Resource.Type)
                    {
                        case SoundBankType.FmodMaster:
                            component.Active = true;
//                            GameEngine.SoundEngine.UnmuteAudio();
                            break;
                        case SoundBankType.FmodStrings:
                            component.Active = true;
                            break;
                    }

                    GameEngine.SoundEngine.TrackSoundBank(component);

                    return component;                    
                }
                default:
                    return null;
            }
        }
    }
}