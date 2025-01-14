using System.Xml.Linq;
using DecayEngine.DecPakLib.Resource;
using DecayEngine.DecPakLib.Resource.Expression.Query.Collection.Terminator;

namespace DecayEngine.ResourceBuilderLib.Resource.Serializers.Expression.Query.Collection.Terminator
{
    public class SelectFirstTerminatorSerializer : IResourceSerializer<SelectFirstCollectionTerminatorExpression>
    {
        public string Tag => "first";

        IResource IResourceSerializer.Deserialize(XElement element)
        {
            return Deserialize(element);
        }

        public XElement Serialize(IResource resource)
        {
            if (!(resource is SelectFirstCollectionTerminatorExpression specificResource)) return null;
            return Serialize(specificResource);
        }

        public XElement Serialize(SelectFirstCollectionTerminatorExpression resource)
        {
            XElement element = new XElement(Tag);

            return element;
        }

        public SelectFirstCollectionTerminatorExpression Deserialize(XElement element)
        {
            if (element.Name != Tag) return null;

            SelectFirstCollectionTerminatorExpression resource = new SelectFirstCollectionTerminatorExpression();

            return resource;
        }

        public bool BuildsType<TResource>()
            where TResource : IResource
        {
            return typeof(TResource).IsAssignableFrom(typeof(SelectFirstCollectionTerminatorExpression));
        }
    }
}