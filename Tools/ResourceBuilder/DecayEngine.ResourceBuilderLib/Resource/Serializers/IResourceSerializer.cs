using System.Xml.Linq;
using DecayEngine.DecPakLib.Resource;

namespace DecayEngine.ResourceBuilderLib.Resource.Serializers
{
    public interface IResourceSerializer
    {
        string Tag { get; }

        IResource Deserialize(XElement element);
        XElement Serialize(IResource resource);

        bool BuildsType<TResource>()
            where TResource : IResource;
    }

    public interface IResourceSerializer<TResource> : IResourceSerializer
        where TResource : IResource
    {
        new TResource Deserialize(XElement element);
        XElement Serialize(TResource resource);
    }
}