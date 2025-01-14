using System.Xml.Linq;
using DecayEngine.DecPakLib.Resource;
using DecayEngine.DecPakLib.Resource.Expression.Query.Collection.Terminator;

namespace DecayEngine.ResourceBuilderLib.Resource.Serializers.Expression.Query.Collection.Terminator
{
    public class SelectAllTerminatorSerializer : IResourceSerializer<SelectAllCollectionTerminatorExpression>
    {
        public string Tag => "first";

        IResource IResourceSerializer.Deserialize(XElement element)
        {
            return Deserialize(element);
        }

        public XElement Serialize(IResource resource)
        {
            if (!(resource is SelectAllCollectionTerminatorExpression specificResource)) return null;
            return Serialize(specificResource);
        }

        public XElement Serialize(SelectAllCollectionTerminatorExpression resource)
        {
            XElement element = new XElement(Tag);

            return element;
        }

        public SelectAllCollectionTerminatorExpression Deserialize(XElement element)
        {
            if (element.Name != Tag) return null;

            SelectAllCollectionTerminatorExpression resource = new SelectAllCollectionTerminatorExpression();

            return resource;
        }

        public bool BuildsType<TResource>()
            where TResource : IResource
        {
            return typeof(TResource).IsAssignableFrom(typeof(SelectFirstCollectionTerminatorExpression));
        }
    }
}