using System.Xml.Linq;
using DecayEngine.DecPakLib.Resource;
using DecayEngine.DecPakLib.Resource.Structure.Math;

namespace DecayEngine.ResourceBuilderLib.Resource.Serializers.Structure.Math
{
    public class TriangleSerializer : IResourceSerializer<TriangleStructure>
    {
        public string Tag => "triangle";

        IResource IResourceSerializer.Deserialize(XElement element)
        {
            return Deserialize(element);
        }

        public XElement Serialize(IResource resource)
        {
            if (!(resource is TriangleStructure specificResource)) return null;
            return Serialize(specificResource);
        }

        public XElement Serialize(TriangleStructure resource)
        {
            XElement element = new XElement(Tag);
            element.SetAttributeValue("v1", resource.Vertex1);
            element.SetAttributeValue("v2", resource.Vertex2);
            element.SetAttributeValue("v3", resource.Vertex3);

            return element;
        }

        public TriangleStructure Deserialize(XElement element)
        {
            TriangleStructure resource = new TriangleStructure();

            if (int.TryParse(element.Attribute("v1")?.Value, out int v1))
            {
                resource.Vertex1 = v1;
            }
            else
            {
                resource.Vertex1 = 0;
            }

            if (int.TryParse(element.Attribute("v2")?.Value, out int v2))
            {
                resource.Vertex2 = v2;
            }
            else
            {
                resource.Vertex2 = 0;
            }

            if (int.TryParse(element.Attribute("v3")?.Value, out int v3))
            {
                resource.Vertex3 = v3;
            }
            else
            {
                resource.Vertex3 = 0;
            }

            return resource;
        }

        public bool BuildsType<TResource>()
            where TResource : IResource
        {
            return typeof(TResource).IsAssignableFrom(typeof(TriangleStructure));
        }
    }
}