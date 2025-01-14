using System.Xml.Linq;
using DecayEngine.DecPakLib.Pointer;
using DecayEngine.DecPakLib.Resource;
using DecayEngine.DecPakLib.Resource.RootElement.Texture3D;

namespace DecayEngine.ResourceBuilderLib.Resource.Serializers.RootElement.Texture3D
{
    public class Texture3DSerializer : IResourceSerializer<Texture3DResource>
    {
        public string Tag => "texture3d";

        IResource IResourceSerializer.Deserialize(XElement element)
        {
            return Deserialize(element);
        }

        public XElement Serialize(IResource resource)
        {
            if (!(resource is Texture3DResource specificResource)) return null;
            return Serialize(specificResource);
        }

        public XElement Serialize(Texture3DResource resource)
        {
            XElement element = new XElement(Tag);
            element.SetAttributeValue("id", resource.Id);

            XElement propertiesElement = new XElement("properties");

            XElement sourceBackElement = new XElement("back", resource.SourceBack.SourcePath);
            propertiesElement.Add(sourceBackElement);

            XElement sourceDownElement = new XElement("down", resource.SourceDown.SourcePath);
            propertiesElement.Add(sourceDownElement);

            XElement sourceFrontElement = new XElement("front", resource.SourceFront.SourcePath);
            propertiesElement.Add(sourceFrontElement);

            XElement sourceLeftElement = new XElement("left", resource.SourceLeft.SourcePath);
            propertiesElement.Add(sourceLeftElement);

            XElement sourceRightElement = new XElement("right", resource.SourceRight.SourcePath);
            propertiesElement.Add(sourceRightElement);

            XElement sourceUpElement = new XElement("up", resource.SourceUp.SourcePath);
            propertiesElement.Add(sourceUpElement);

            element.Add(propertiesElement);

            return element;
        }

        public Texture3DResource Deserialize(XElement element)
        {
            if (element.Name != Tag) return null;

            Texture3DResource resource = new Texture3DResource
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
                                case "back":
                                    resource.SourceBack = new DataPointer
                                    {
                                        SourcePath = property.Value
                                    };
                                    break;
                                case "down":
                                    resource.SourceDown = new DataPointer
                                    {
                                        SourcePath = property.Value
                                    };
                                    break;
                                case "front":
                                    resource.SourceFront = new DataPointer
                                    {
                                        SourcePath = property.Value
                                    };
                                    break;
                                case "left":
                                    resource.SourceLeft = new DataPointer
                                    {
                                        SourcePath = property.Value
                                    };
                                    break;
                                case "right":
                                    resource.SourceRight = new DataPointer
                                    {
                                        SourcePath = property.Value
                                    };
                                    break;
                                case "up":
                                    resource.SourceUp = new DataPointer
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
            return typeof(TResource).IsAssignableFrom(typeof(Texture3DResource));
        }
    }
}