using System.Globalization;
using System.Xml.Linq;
using DecayEngine.DecPakLib.Resource;
using DecayEngine.DecPakLib.Resource.Structure.Math;

namespace DecayEngine.ResourceBuilderLib.Resource.Serializers.Structure.Math
{
    public class Vector4Serializer : IResourceSerializer<Vector4Structure>
    {
        public string Tag => "vector4";

        IResource IResourceSerializer.Deserialize(XElement element)
        {
            return Deserialize(element);
        }

        public XElement Serialize(IResource resource)
        {
            if (!(resource is Vector4Structure specificResource)) return null;
            return Serialize(specificResource);
        }

        public XElement Serialize(Vector4Structure resource)
        {
            XElement element = new XElement(Tag);
            element.SetAttributeValue("x", resource.X);
            element.SetAttributeValue("y", resource.Y);
            element.SetAttributeValue("z", resource.Z);
            element.SetAttributeValue("w", resource.W);

            return element;
        }

        public Vector4Structure Deserialize(XElement element)
        {
            Vector4Structure resource = new Vector4Structure();

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

            if (float.TryParse(element.Attribute("w")?.Value, NumberStyles.Any, CultureInfo.InvariantCulture, out float w))
            {
                resource.W = w;
            }
            else
            {
                resource.W = 0;
            }

            return resource;
        }

        public bool BuildsType<TResource>()
            where TResource : IResource
        {
            return typeof(TResource).IsAssignableFrom(typeof(Vector4Structure));
        }
    }
}