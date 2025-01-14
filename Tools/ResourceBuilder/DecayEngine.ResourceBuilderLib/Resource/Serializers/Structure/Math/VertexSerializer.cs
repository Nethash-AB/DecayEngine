using System.Globalization;
using System.Xml.Linq;
using DecayEngine.DecPakLib.Resource;
using DecayEngine.DecPakLib.Resource.Structure.Math;

namespace DecayEngine.ResourceBuilderLib.Resource.Serializers.Structure.Math
{
    public class VertexSerializer : IResourceSerializer<VertexStructure>
    {
        public string Tag => "vertex";

        IResource IResourceSerializer.Deserialize(XElement element)
        {
            return Deserialize(element);
        }

        public XElement Serialize(IResource resource)
        {
            if (!(resource is VertexStructure specificResource)) return null;
            return Serialize(specificResource);
        }

        public XElement Serialize(VertexStructure resource)
        {
            XElement element = new XElement(Tag);
            element.SetAttributeValue("x", resource.X);
            element.SetAttributeValue("y", resource.Y);
            element.SetAttributeValue("z", resource.Z);
            element.SetAttributeValue("u", resource.U);
            element.SetAttributeValue("v", resource.V);

            return element;
        }

        public VertexStructure Deserialize(XElement element)
        {
            VertexStructure resource = new VertexStructure();

            if (float.TryParse(element.Attribute("x")?.Value, NumberStyles.Any, CultureInfo.InvariantCulture, out float x))
            {
                resource.X = x;
            }
            else
            {
                resource.X = 0;
            }

            if (float.TryParse(element.Attribute("y")?.Value, NumberStyles.Any, CultureInfo.InvariantCulture, out float y))
            {
                resource.Y = y;
            }
            else
            {
                resource.Y = 0;
            }

            if (float.TryParse(element.Attribute("z")?.Value, NumberStyles.Any, CultureInfo.InvariantCulture, out float z))
            {
                resource.Z = z;
            }
            else
            {
                resource.Z = 0;
            }

            if (float.TryParse(element.Attribute("u")?.Value, NumberStyles.Any, CultureInfo.InvariantCulture, out float u))
            {
                resource.U = u;
            }
            else
            {
                resource.U = 0;
            }

            if (float.TryParse(element.Attribute("v")?.Value, NumberStyles.Any, CultureInfo.InvariantCulture, out float v))
            {
                resource.V = v;
            }
            else
            {
                resource.V = 0;
            }

            return resource;
        }

        public bool BuildsType<TResource>()
            where TResource : IResource
        {
            return typeof(TResource).IsAssignableFrom(typeof(VertexStructure));
        }
    }
}