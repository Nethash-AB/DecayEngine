using System.Xml.Linq;
using DecayEngine.DecPakLib.Resource;
using DecayEngine.DecPakLib.Resource.Expression.Property.Reference;

namespace DecayEngine.ResourceBuilderLib.Resource.Serializers.Expression.Property.Reference
{
    public class ResourceReferenceExpressionSerializer : IResourceSerializer<ResourceReferenceExpression>
    {
        public string Tag => "resource";

        IResource IResourceSerializer.Deserialize(XElement element)
        {
            return Deserialize(element);
        }

        public XElement Serialize(IResource resource)
        {
            if (!(resource is ResourceReferenceExpression specificResource)) return null;
            return Serialize(specificResource);
        }

        public XElement Serialize(ResourceReferenceExpression resource)
        {
            XElement element = new XElement(Tag);
            element.SetAttributeValue("id", resource.ReferenceId);

            return element;
        }

        public ResourceReferenceExpression Deserialize(XElement element)
        {
            if (element.Name != Tag) return null;

            ResourceReferenceExpression resource = new ResourceReferenceExpression
            {
                ReferenceId = element.Attribute("id")?.Value
            };

            return resource;
        }

        public bool BuildsType<TResource>()
            where TResource : IResource
        {
            return typeof(TResource).IsAssignableFrom(typeof(ResourceReferenceExpression));
        }
    }
}