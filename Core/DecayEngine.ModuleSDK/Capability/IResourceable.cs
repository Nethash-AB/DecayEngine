using DecayEngine.DecPakLib.Resource.RootElement;

namespace DecayEngine.ModuleSDK.Capability
{
    public interface IResourceable<out TResource> where TResource : IRootResource
    {
        TResource Resource { get; }
    }
}