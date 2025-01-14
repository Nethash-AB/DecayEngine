using System.Linq;
using System.Xml.Linq;
using DecayEngine.DecPakLib.Resource;
using DecayEngine.DecPakLib.Resource.Expression.Property;
using DecayEngine.DecPakLib.Resource.RootElement.Sound;

namespace DecayEngine.ResourceBuilderLib.Resource.Serializers.RootElement.Sound
{
    public class SoundSerializer : IResourceSerializer<SoundResource>
    {
        public string Tag => "sound";

        IResource IResourceSerializer.Deserialize(XElement element)
        {
            return Deserialize(element);
        }

        public XElement Serialize(IResource resource)
        {
            if (!(resource is SoundResource specificResource)) return null;
            return Serialize(specificResource);
        }

        public XElement Serialize(SoundResource resource)
        {
            XElement element = new XElement(Tag);
            element.SetAttributeValue("id", resource.Id);

            XElement propertiesElement = new XElement("properties");
            XElement bankElement = new XElement("bank");

            if (resource.Bank != null)
            {
                IResourceSerializer serializer = ResourceSerializationController.GetSerializer(resource.Bank);
                if (serializer != null && serializer.BuildsType<IPropertyExpression>())
                {
                    XElement expressionElement = serializer.Serialize(resource.Bank);
                    if (expressionElement != null)
                    {
                        bankElement.Add(expressionElement);
                    }
                }
            }

            if (bankElement.HasElements)
            {
                propertiesElement.Add(bankElement);
            }

            if (!string.IsNullOrEmpty(resource.Event))
            {
                XElement eventElement = new XElement("event", resource.Event);
                propertiesElement.Add(eventElement);
            }

            element.Add(propertiesElement);

            return element;
        }

        public SoundResource Deserialize(XElement element)
        {
            if (element.Name != Tag) return null;

            SoundResource resource = new SoundResource
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
                                case "bank":
                                    XElement expressionElement = property.Elements().FirstOrDefault();
                                    if (expressionElement == null) continue;

                                    IResourceSerializer serializer = ResourceSerializationController.GetSerializer(expressionElement.Name.ToString());
                                    IPropertyExpression expression = (IPropertyExpression) serializer?.Deserialize(expressionElement);
                                    if (expression == null) continue;

                                    resource.Bank = expression;

                                    break;
                                case "event":
                                    resource.Event = property.Value;
                                    break;
                            }
                        }
                        break;
                }
            }

            if (string.IsNullOrEmpty(resource.Event) || resource.Bank == null)
            {
                return null;
            }

            return resource;
        }

        public bool BuildsType<TResource>()
            where TResource : IResource
        {
            return typeof(TResource).IsAssignableFrom(typeof(SoundResource));
        }
    }
}