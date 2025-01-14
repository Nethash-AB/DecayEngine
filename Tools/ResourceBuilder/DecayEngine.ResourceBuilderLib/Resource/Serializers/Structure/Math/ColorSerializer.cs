using System.Globalization;
using System.Xml.Linq;
using DecayEngine.DecPakLib.Resource;
using DecayEngine.DecPakLib.Resource.Structure.Math;

namespace DecayEngine.ResourceBuilderLib.Resource.Serializers.Structure.Math
{
    public class ColorSerializer : IResourceSerializer<ColorStructure>
    {
        public string Tag => "color";

        IResource IResourceSerializer.Deserialize(XElement element)
        {
            return Deserialize(element);
        }

        public XElement Serialize(IResource resource)
        {
            if (!(resource is ColorStructure specificResource)) return null;
            return Serialize(specificResource);
        }

        public XElement Serialize(ColorStructure resource)
        {
            XElement element = new XElement(Tag);
            if (resource.WasStatic)
            {
                element.SetValue(resource.ColorName);
            }
            else
            {
                element.SetAttributeValue("r", resource.R);
                element.SetAttributeValue("g", resource.G);
                element.SetAttributeValue("b", resource.B);
                element.SetAttributeValue("a", resource.A);
            }

            return element;
        }

        public ColorStructure Deserialize(XElement element)
        {
            ColorStructure resource;

            if (!string.IsNullOrEmpty(element.Value))
            {
                resource = ColorStructure.FromColorName(element.Value);
            }
            else
            {
                if (float.TryParse(element.Attribute("r")?.Value, NumberStyles.Any, CultureInfo.InvariantCulture, out float r))
                {
                    if (r > 1f)
                    {
                        r /= 255f;
                    }
                    else if (r < 0f)
                    {
                        r = 0f;
                    }
                }
                else
                {
                    r = 0;
                }

                if (float.TryParse(element.Attribute("g")?.Value, NumberStyles.Any, CultureInfo.InvariantCulture, out float g))
                {
                    if (g > 1f)
                    {
                        g /= 255f;
                    }
                    else if (g < 0f)
                    {
                        g = 0f;
                    }
                }
                else
                {
                    g = 0;
                }

                if (float.TryParse(element.Attribute("b")?.Value, NumberStyles.Any, CultureInfo.InvariantCulture, out float b))
                {
                    if (b > 1f)
                    {
                        b /= 255f;
                    }
                    else if (b < 0f)
                    {
                        b = 0f;
                    }
                }
                else
                {
                    b = 0;
                }

                if (float.TryParse(element.Attribute("a")?.Value, NumberStyles.Any, CultureInfo.InvariantCulture, out float a))
                {
                    if (a > 1f)
                    {
                        a /= 255f;
                    }
                    else if (a < 0f)
                    {
                        a = 0f;
                    }
                }
                else
                {
                    a = 0;
                }

                resource = new ColorStructure(r, g, b, a);
            }

            return resource;
        }

        public bool BuildsType<TResource>()
            where TResource : IResource
        {
            return typeof(TResource).IsAssignableFrom(typeof(ColorStructure));
        }
    }
}