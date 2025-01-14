using System.Xml.Linq;
using DecayEngine.DecPakLib.Resource;
using DecayEngine.DecPakLib.Resource.Expression.Query.Collection.Terminator;

namespace DecayEngine.ResourceBuilderLib.Resource.Serializers.Expression.Query.Collection.Terminator
{
    public class SelectFrameBufferTerminatorSerializer : IResourceSerializer<SelectFrameBufferTerminatorExpression>
    {
        public string Tag => "frameBuffer";

        IResource IResourceSerializer.Deserialize(XElement element)
        {
            return Deserialize(element);
        }

        public XElement Serialize(IResource resource)
        {
            if (!(resource is SelectFrameBufferTerminatorExpression specificResource)) return null;
            return Serialize(specificResource);
        }

        public XElement Serialize(SelectFrameBufferTerminatorExpression resource)
        {
            XElement element = new XElement(Tag);
            element.SetAttributeValue("name", resource.Name);

            return element;
        }

        public SelectFrameBufferTerminatorExpression Deserialize(XElement element)
        {
            if (element.Name != Tag) return null;

            SelectFrameBufferTerminatorExpression resource = new SelectFrameBufferTerminatorExpression
            {
                Name = element.Attribute("name")?.Value
            };

            return resource;
        }

        public bool BuildsType<TResource>()
            where TResource : IResource
        {
            return typeof(TResource).IsAssignableFrom(typeof(SelectFirstCollectionTerminatorExpression));
        }
    }
}