using System.Globalization;
using System.Xml.Linq;
using DecayEngine.DecPakLib.Resource;
using DecayEngine.DecPakLib.Resource.Structure.Math;

namespace DecayEngine.ResourceBuilderLib.Resource.Serializers.Structure.Math
{
    public class Vector2Serializer : IResourceSerializer<Vector2Structure>
    {
        public string Tag => "vector2";

        IResource IResourceSerializer.Deserialize(XElement element)
        {
            return Deserialize(element);
        }

        public XElement Serialize(IResource resource)
        {
            if (!(resource is Vector2Structure specificResource)) return null;
            return Serialize(specificResource);
        }

        public XElement Serialize(Vector2Structure resource)
        {
            XElement element = new XElement(Tag);
            element.SetAttributeValue("x", resource.X);
            element.SetAttributeValue("y", resource.Y);

            return element;
        }

        public Vector2Structure Deserialize(XElement element)
        {
            Vector2Structure resource = new Vector2Structure();

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

            return resource;
        }

        public bool BuildsType<TResource>()
            where TResource : IResource
        {
            return typeof(TResource).IsAssignableFrom(typeof(Vector2Structure));
        }
    }
}