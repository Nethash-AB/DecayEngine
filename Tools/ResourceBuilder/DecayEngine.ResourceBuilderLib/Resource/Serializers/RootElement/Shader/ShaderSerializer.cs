using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using DecayEngine.DecPakLib.Pointer;
using DecayEngine.DecPakLib.Resource;
using DecayEngine.DecPakLib.Resource.RootElement.Shader;

namespace DecayEngine.ResourceBuilderLib.Resource.Serializers.RootElement.Shader
{
    public class ShaderSerializer : IResourceSerializer<ShaderResource>
    {
        public string Tag => "shader";

        IResource IResourceSerializer.Deserialize(XElement element)
        {
            return Deserialize(element);
        }

        public XElement Serialize(IResource resource)
        {
            if (!(resource is ShaderResource specificResource)) return null;
            return Serialize(specificResource);
        }

        public XElement Serialize(ShaderResource resource)
        {
            XElement element = new XElement(Tag);
            element.SetAttributeValue("id", resource.Id);

            XElement propertiesElement = new XElement("properties");

            XElement languageElement = new XElement("language", resource.Language.ToString().ToLower());
            propertiesElement.Add(languageElement);

            if (resource.FallbackTargets != null && resource.FallbackTargets.Count > 0)
            {
                XElement targetElement = new XElement("fallbacks", string.Join(";", resource.FallbackTargets.Select(f => f.ToString().ToLower())));
                propertiesElement.Add(targetElement);
            }

            XElement compileElement = new XElement("compile", resource.Compile);
            propertiesElement.Add(compileElement);

            XElement typeElement = new XElement("type", resource.Type.ToString().ToLower());
            propertiesElement.Add(typeElement);

            XElement sourceElement = new XElement("source", resource.Source.SourcePath);
            propertiesElement.Add(sourceElement);

            element.Add(propertiesElement);

            return element;
        }

        public ShaderResource Deserialize(XElement element)
        {
            if (element.Name != Tag) return null;

            ShaderResource resource = new ShaderResource
            {
                Id = element.Attribute("id")?.Value,
                MetaFilePath = element.Attribute("filePath")?.Value,
                FallbackTargets = new List<ShaderLanguage>()
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
                                case "language":
                                    resource.Language = (ShaderLanguage) Enum.Parse(typeof(ShaderLanguage), property.Value, true);
                                    break;
                                case "fallbacks":
                                    resource.FallbackTargets = new List<ShaderLanguage>();
                                    foreach (string value in property.Value.Split(';'))
                                    {
                                        if (Enum.TryParse(value, true, out ShaderLanguage fallbackTarget))
                                        {
                                            resource.FallbackTargets.Add(fallbackTarget);
                                        }
                                    }
                                    break;
                                case "compile":
                                    resource.Compile = bool.TryParse(property.Value, out bool result) && result;
                                    break;
                                case "type":
                                    resource.Type = (ShaderType) Enum.Parse(typeof(ShaderType), property.Value, true);
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
            return typeof(TResource).IsAssignableFrom(typeof(ShaderResource));
        }
    }
}