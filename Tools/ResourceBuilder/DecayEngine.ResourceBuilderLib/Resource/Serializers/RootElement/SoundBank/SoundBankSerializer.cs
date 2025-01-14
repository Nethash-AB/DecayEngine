using System;
using System.Collections.Generic;
using System.Xml.Linq;
using DecayEngine.DecPakLib.Pointer;
using DecayEngine.DecPakLib.Resource;
using DecayEngine.DecPakLib.Resource.Expression.Property;
using DecayEngine.DecPakLib.Resource.RootElement.SoundBank;

namespace DecayEngine.ResourceBuilderLib.Resource.Serializers.RootElement.SoundBank
{
    public class SoundBankSerializer : IResourceSerializer<SoundBankResource>
    {
        public string Tag => "soundBank";

        IResource IResourceSerializer.Deserialize(XElement element)
        {
            return Deserialize(element);
        }

        public XElement Serialize(IResource resource)
        {
            if (!(resource is SoundBankResource specificResource)) return null;
            return Serialize(specificResource);
        }

        public XElement Serialize(SoundBankResource resource)
        {
            XElement element = new XElement(Tag);
            element.SetAttributeValue("id", resource.Id);

            XElement propertiesElement = new XElement("properties");

            XElement typeElement = new XElement("type", resource.Type.ToString().ToLower());
            propertiesElement.Add(typeElement);

            XElement sourceElement = new XElement("source", resource.Source.SourcePath);
            propertiesElement.Add(sourceElement);

            if (resource.Requires != null)
            {
                XElement requiresElement = new XElement("requires");
                foreach (IPropertyExpression requireExpression in resource.Requires)
                {
                    IResourceSerializer serializer = ResourceSerializationController.GetSerializer(requireExpression);
                    if (serializer != null && serializer.BuildsType<IPropertyExpression>())
                    {
                        XElement expressionElement = serializer.Serialize(requireExpression);
                        if (expressionElement != null)
                        {
                            requiresElement.Add(expressionElement);
                        }
                    }
                }

                if (requiresElement.HasElements)
                {
                    propertiesElement.Add(requiresElement);
                }
            }

            element.Add(propertiesElement);

            return element;
        }

        public SoundBankResource Deserialize(XElement element)
        {
            if (element.Name != Tag) return null;

            SoundBankResource resource = new SoundBankResource
            {
                Id = element.Attribute("id")?.Value,
                MetaFilePath = element.Attribute("filePath")?.Value,
                Requires = new List<IPropertyExpression>()
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
                                    resource.Type = (SoundBankType) Enum.Parse(typeof(SoundBankType), property.Value, true);
                                    break;
                                case "source":
                                    resource.Source = new DataPointer
                                    {
                                        SourcePath = property.Value
                                    };
                                    break;
                                case "requires":
                                    foreach (XElement expressionElement in property.Elements())
                                    {
                                        IResourceSerializer serializer = ResourceSerializationController.GetSerializer(expressionElement.Name.ToString());
                                        IPropertyExpression expression = (IPropertyExpression) serializer?.Deserialize(expressionElement);
                                        if (expression == null) continue;

                                        resource.Requires.Add(expression);
                                    }
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
            return typeof(TResource).IsAssignableFrom(typeof(SoundBankResource));
        }
    }
}