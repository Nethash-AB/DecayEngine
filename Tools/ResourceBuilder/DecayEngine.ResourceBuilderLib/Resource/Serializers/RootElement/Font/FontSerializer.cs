using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using DecayEngine.DecPakLib.Pointer;
using DecayEngine.DecPakLib.Resource;
using DecayEngine.DecPakLib.Resource.RootElement.Font;
using DecayEngine.DecPakLib.Resource.Structure.Component.Font;

namespace DecayEngine.ResourceBuilderLib.Resource.Serializers.RootElement.Font
{
    public class FontSerializer : IResourceSerializer<FontResource>
    {
        public string Tag => "font";

        IResource IResourceSerializer.Deserialize(XElement element)
        {
            return Deserialize(element);
        }

        public XElement Serialize(IResource resource)
        {
            if (!(resource is FontResource specificResource)) return null;
            return Serialize(specificResource);
        }

        public XElement Serialize(FontResource resource)
        {
            XElement element = new XElement(Tag);
            element.SetAttributeValue("id", resource.Id);

            XElement propertiesElement = new XElement("properties");

            XElement extraCharactersElement = new XElement("extraCharacters", resource.ExtraCharacters);
            propertiesElement.Add(extraCharactersElement);

            if (resource.OutlineOnly)
            {
                XElement outlineElement = new XElement("outline", resource.ExtraCharacters);

                XElement outlineTicknessElement = new XElement("tickness", resource.OutlineThickness);
                outlineElement.Add(outlineTicknessElement);

                propertiesElement.Add(outlineElement);
            }

            XElement sourceElement = new XElement("source", resource.Source.SourcePath);
            propertiesElement.Add(sourceElement);

            element.Add(propertiesElement);

            return element;
        }

        public FontResource Deserialize(XElement element)
        {
            if (element.Name != Tag) return null;

            FontResource resource = new FontResource
            {
                Id = element.Attribute("id")?.Value,
                MetaFilePath = element.Attribute("filePath")?.Value,
                ExtraCharacters = new List<string>(),
                Glyphs = new List<Glyph>(),
                MipMaps = new List<FontMipMap>()
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
                                case "extraCharacters":
                                    resource.ExtraCharacters = property.Value.Split().Select(s => s.TrimEnd().TrimStart()).ToList();
                                    break;
                                case "outline":
                                    XElement thicknessElement = property.Elements().FirstOrDefault(el => el.Name == "tickness");
                                    if (thicknessElement == null) continue;

                                    resource.OutlineOnly = true;
                                    resource.OutlineThickness = float.Parse(thicknessElement.Value, NumberStyles.Any, CultureInfo.InvariantCulture);
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
            return typeof(TResource).IsAssignableFrom(typeof(FontResource));
        }
    }
}