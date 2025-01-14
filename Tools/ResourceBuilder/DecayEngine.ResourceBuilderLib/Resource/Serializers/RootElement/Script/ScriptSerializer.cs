using System;
using System.Xml.Linq;
using DecayEngine.DecPakLib.Pointer;
using DecayEngine.DecPakLib.Resource;
using DecayEngine.DecPakLib.Resource.RootElement.Script;

namespace DecayEngine.ResourceBuilderLib.Resource.Serializers.RootElement.Script
{
    public class ScriptSerializer : IResourceSerializer<ScriptResource>
    {
        public string Tag => "script";

        IResource IResourceSerializer.Deserialize(XElement element)
        {
            return Deserialize(element);
        }

        public XElement Serialize(IResource resource)
        {
            if (!(resource is ScriptResource specificResource)) return null;
            return Serialize(specificResource);
        }

        public XElement Serialize(ScriptResource resource)
        {
            XElement element = new XElement(Tag);
            element.SetAttributeValue("id", resource.Id);

            XElement propertiesElement = new XElement("properties");

            XElement typeElement = new XElement("type", resource.Type.ToString().ToLower());
            propertiesElement.Add(typeElement);

            XElement sourceElement = new XElement("source", resource.Source.SourcePath);
            propertiesElement.Add(sourceElement);

            element.Add(propertiesElement);

            return element;
        }

        public ScriptResource Deserialize(XElement element)
        {
            if (element.Name != Tag) return null;

            ScriptResource resource = new ScriptResource
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
                                case "type":
                                    resource.Type = (ScriptType) Enum.Parse(typeof(ScriptType), property.Value, true);
                                    break;
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
            return typeof(TResource).IsAssignableFrom(typeof(ScriptResource));
        }
    }
}