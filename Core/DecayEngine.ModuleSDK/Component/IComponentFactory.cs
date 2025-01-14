using DecayEngine.DecPakLib.Resource.RootElement;

namespace DecayEngine.ModuleSDK.Component
{
    public interface IComponentFactory
    {
    }

    public interface IComponentFactory<out TComponent> : IComponentFactory
        where TComponent : IComponent
    {
        TComponent CreateComponent();
    }

    public interface IComponentFactory<out TComponent, in TResource> : IComponentFactory
        where TComponent : IComponent
        where TResource : IRootResource
    {
        TComponent CreateComponent(TResource resource); // IF THIS IS RENAMED MAKE SURE IT'S ALSO RENAMED AT GameEngineImpl!!!!!!
    }
}