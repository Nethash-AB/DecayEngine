using DecayEngine.DecPakLib.Resource.RootElement.Prefab;
using DecayEngine.ModuleSDK.Capability;
using DecayEngine.ModuleSDK.Object.GameStructure;
using DecayEngine.ModuleSDK.Object.Scene;

namespace DecayEngine.ModuleSDK.Object.GameObject
{
    public interface IGameObject : IGameStructure, ITransformable, INameable, IParentable<IGameObject>, IParentable<IScene>, IComponentable, IDestroyable, IResourceable<PrefabResource>
    {
        bool Persistent { get; set; }
    }
}