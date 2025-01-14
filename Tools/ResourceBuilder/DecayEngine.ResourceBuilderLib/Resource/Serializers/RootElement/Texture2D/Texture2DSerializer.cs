using System.Xml.Linq;
using DecayEngine.DecPakLib.Pointer;
using DecayEngine.DecPakLib.Resource;
using DecayEngine.DecPakLib.Resource.RootElement.Texture2D;

namespace DecayEngine.ResourceBuilderLib.Resource.Serializers.RootElement.Texture2D
{
    public class Texture2DSerializer : IResourceSerializer<Texture2DResource>
    {
        public string Tag => "texture2d";

        IResource IResourceSerializer.Deserialize(XElement element)
        {
            return Deserialize(element);
        }

        public XElement Serialize(IResource resource)
        {
            if (!(resource is Texture2DResource specificResource)) return null;
            return Serialize(specificResource);
        }

        public XElement Serialize(Texture2DResource resource)
        {
            XElement element = new XElement(Tag);
            element.SetAttributeValue("id", resource.Id);

            XElement propertiesElement = new XElement("properties");

            XElement sourceElement = new XElement("source", resource.Source.SourcePath);
            propertiesElement.Add(sourceElement);

            element.Add(propertiesElement);

            return element;
        }

        public Texture2DResource Deserialize(XElement element)
        {
            if (element.Name != Tag) return null;

            Texture2DResource resource = new Texture2DResource
            {
                Id = element.Attribute("id")?.Value,
                MetaFilePath = element.Attribute("filePath")?.Value
            };

            foreach (XElement child in element.Elements())
            {
                switch (child.Name.ToString())
                {
                    case "properties":
                        foreach (XElement property in child.Elements())
                        {
                            switch (property.Name.ToString())
                            {
                                case "source":
                                    resource.Source = new DataPointer
                                    {
                                        SourcePath = property.Value
                                    };
                                    break;
                            }
                        }
                        break;
                }
            }

            return resource;
        }

        public bool BuildsType<TResource>()
            where TResource : IResource
        {
            return typeof(TResource).IsAssignableFrom(typeof(Texture2DResource));
        }
    }
}