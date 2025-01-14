using DecayEngine.ModuleSDK.Capability;
using DecayEngine.ModuleSDK.Object.Scene;

namespace DecayEngine.ModuleSDK.Component
{
    public interface ISceneAttachableComponent : IComponent, IParentable<IScene>
    {
        bool Persistent { get; set; }
    }
}