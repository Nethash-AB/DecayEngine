using System;
using DecayEngine.DecPakLib.Resource.RootElement;
using DecayEngine.ModuleSDK.Capability;
using DecayEngine.ModuleSDK.Object;
using DecayEngine.ModuleSDK.Object.GameObject;

namespace DecayEngine.ModuleSDK.Component
{
    public interface IComponent : IActivable, INameable, IDestroyable, IParentable<IGameObject>
    {
        Type ExportType { get; }
    }

    public interface IComponent<out TResource> : IComponent, IResourceable<TResource> where TResource : IRootResource
    {
    }
}