using System.Globalization;
using System.Xml.Linq;
using DecayEngine.DecPakLib.Resource;
using DecayEngine.DecPakLib.Resource.Structure.Math;

namespace DecayEngine.ResourceBuilderLib.Resource.Serializers.Structure.Math
{
    public class Vector3Serializer : IResourceSerializer<Vector3Structure>
    {
        public string Tag => "vector3";

        IResource IResourceSerializer.Deserialize(XElement element)
        {
            return Deserialize(element);
        }

        public XElement Serialize(IResource resource)
        {
            if (!(resource is Vector3Structure specificResource)) return null;
            return Serialize(specificResource);
        }

        public XElement Serialize(Vector3Structure resource)
        {
            XElement element = new XElement(Tag);
            element.SetAttributeValue("x", resource.X);
            element.SetAttributeValue("y", resource.Y);
            element.SetAttributeValue("z", resource.Z);

            return element;
        }

        public Vector3Structure Deserialize(XElement element)
        {
            Vector3Structure resource = new Vector3Structure();

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

            return resource;
        }

        public bool BuildsType<TResource>()
            where TResource : IResource
        {
            return typeof(TResource).IsAssignableFrom(typeof(Vector3Structure));
        }
    }
}