using DecayEngine.DecPakLib.Resource.RootElement.Scene;
using DecayEngine.ModuleSDK.Capability;
using DecayEngine.ModuleSDK.Component;
using DecayEngine.ModuleSDK.Object.GameStructure;

namespace DecayEngine.ModuleSDK.Object.Scene
{
    public interface IScene : IGameStructure, IComponentable<ISceneAttachableComponent>
    {
        string Name { get; }
        SceneResource Resource { get; }
    }
}