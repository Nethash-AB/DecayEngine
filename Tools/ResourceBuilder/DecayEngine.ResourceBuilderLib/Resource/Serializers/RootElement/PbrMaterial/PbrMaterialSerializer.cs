using System.Globalization;
using System.Xml.Linq;
using DecayEngine.DecPakLib.Resource;
using DecayEngine.DecPakLib.Resource.Expression.Property;
using DecayEngine.DecPakLib.Resource.RootElement.PbrMaterial;
using DecayEngine.DecPakLib.Resource.Structure.Math;

namespace DecayEngine.ResourceBuilderLib.Resource.Serializers.RootElement.PbrMaterial
{
    public class PbrMaterialSerializer : IResourceSerializer<PbrMaterialResource>
    {
        public string Tag => "pbrMaterial";

        IResource IResourceSerializer.Deserialize(XElement element)
        {
            return Deserialize(element);
        }

        public XElement Serialize(IResource resource)
        {
            if (!(resource is PbrMaterialResource specificResource)) return null;
            return Serialize(specificResource);
        }

        public XElement Serialize(PbrMaterialResource resource)
        {
            XElement element = new XElement(Tag);
            element.SetAttributeValue("id", resource.Id);

            XElement propertiesElement = new XElement("properties");
            XElement texturesElement = new XElement("textures");

            IResourceSerializer<Vector4Structure> vector4Serializer = ResourceSerializationController.GetSerializer<Vector4Structure>();

            // Albedo
            XElement albedoElement = new XElement("albedo");

            IResourceSerializer serializer = ResourceSerializationController.GetSerializer(resource.AlbedoTexture);
            if (serializer != null && serializer.BuildsType<IPropertyExpression>())
            {
                XElement expressionElement = serializer.Serialize(resource.AlbedoTexture);
                if (expressionElement != null)
                {
                    albedoElement.Add(expressionElement);
                }
            }

            if (vector4Serializer != null)
            {
                XElement albedoColorElement = vector4Serializer.Serialize(resource.AlbedoColor);
                albedoColorElement.Name = "color";
                albedoElement.Add(albedoColorElement);
            }

            if (albedoElement.HasElements)
            {
                texturesElement.Add(albedoElement);
            }

            // Normal
            XElement normalElement = new XElement("normal");

            serializer = ResourceSerializationController.GetSerializer(resource.NormalTexture);
            if (serializer != null && serializer.BuildsType<IPropertyExpression>())
            {
                XElement expressionElement = serializer.Serialize(resource.NormalTexture);
                if (expressionElement != null)
                {
                    normalElement.Add(expressionElement);
                }
            }

            if (normalElement.HasElements)
            {
                texturesElement.Add(normalElement);
            }

            // Metallicity
            XElement metallicityElement = new XElement("metallicity");

            serializer = ResourceSerializationController.GetSerializer(resource.MetallicityTexture);
            if (serializer != null && serializer.BuildsType<IPropertyExpression>())
            {
                XElement expressionElement = serializer.Serialize(resource.MetallicityTexture);
                if (expressionElement != null)
                {
                    metallicityElement.Add(expressionElement);
                }
            }

            XElement metallicityFactorElement = new XElement("factor", resource.MetallicityFactor);
            metallicityElement.Add(metallicityFactorElement);

            texturesElement.Add(metallicityElement);

            // Metallicity
            XElement roughnessElement = new XElement("roughness");

            serializer = ResourceSerializationController.GetSerializer(resource.RoughnessTexture);
            if (serializer != null && serializer.BuildsType<IPropertyExpression>())
            {
                XElement expressionElement = serializer.Serialize(resource.RoughnessTexture);
                if (expressionElement != null)
                {
                    roughnessElement.Add(expressionElement);
                }
            }

            XElement roughnessFactorElement = new XElement("factor", resource.MetallicityFactor);
            roughnessElement.Add(roughnessFactorElement);

            texturesElement.Add(roughnessElement);

            // Emission
            XElement emissionElement = new XElement("emission");

            serializer = ResourceSerializationController.GetSerializer(resource.EmissionTexture);
            if (serializer != null && serializer.BuildsType<IPropertyExpression>())
            {
                XElement expressionElement = serializer.Serialize(resource.EmissionTexture);
                if (expressionElement != null)
                {
                    emissionElement.Add(expressionElement);
                }
            }

            if (vector4Serializer != null)
            {
                XElement emissionColorElement = vector4Serializer.Serialize(resource.EmissionColor);
                emissionColorElement.Name = "color";
                emissionElement.Add(emissionColorElement);
            }

            if (emissionElement.HasElements)
            {
                texturesElement.Add(emissionElement);
            }

            if (texturesElement.HasElements)
            {
                propertiesElement.Add(texturesElement);
            }

            if (propertiesElement.HasElements)
            {
                element.Add(propertiesElement);
            }

            return element;
        }

        public PbrMaterialResource Deserialize(XElement element)
        {
            if (element.Name != Tag) return null;

            PbrMaterialResource resource = new PbrMaterialResource
            {
                Id = element.Attribute("id")?.Value,
                MetaFilePath = element.Attribute("filePath")?.Value,
                AlbedoColor = new Vector4Structure
                {
                    X = 1f,
                    Y = 1f,
                    Z = 1f,
                    W = 1f,
                    WasNull = true
                },
                MetallicityFactor = 0f,
                RoughnessFactor = 0f,
                EmissionColor = new Vector4Structure
                {
                    X = 1f,
                    Y = 1f,
                    Z = 1f,
                    W = 1f,
                    WasNull = true
                }
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
                                case "textures":
                                    foreach (XElement texture in property.Elements())
                                    {
                                        string textureName = texture.Name.ToString();

                                        foreach (XElement textureChild in texture.Elements())
                                        {
                                            switch (textureChild.Name.ToString())
                                            {
                                                case "color" when textureName == "albedo" || textureName == "emission":
                                                {
                                                    IResourceSerializer<Vector4Structure> vector4Serializer =
                                                        ResourceSerializationController.GetSerializer<Vector4Structure>();

                                                    Vector4Structure color = vector4Serializer?.Deserialize(textureChild);
                                                    if (color == null) continue;

                                                    switch (textureName)
                                                    {
                                                        case "albedo":
                                                            resource.AlbedoColor = color;
                                                            break;
                                                        case "emission":
                                                            resource.EmissionColor = color;
                                                            break;
                                                    }
                                                    break;
                                                }
                                                case "factor" when textureName == "metallicity":
                                                {
                                                    resource.MetallicityFactor =
                                                        float.Parse(textureChild.Value, NumberStyles.Any, CultureInfo.InvariantCulture);
                                                    break;
                                                }
                                                case "factor" when textureName == "roughness":
                                                {
                                                    resource.RoughnessFactor =
                                                        float.Parse(textureChild.Value, NumberStyles.Any, CultureInfo.InvariantCulture);
                                                    break;
                                                }
                                                default:
                                                {
                                                    IResourceSerializer serializer =
                                                        ResourceSerializationController.GetSerializer(textureChild.Name.ToString());
                                                    if (serializer == null || !serializer.BuildsType<IPropertyExpression>()) continue;

                                                    IPropertyExpression expression = (IPropertyExpression) serializer.Deserialize(textureChild);
                                                    if (expression == null) continue;

                                                    switch (textureName)
                                                    {
                                                        case "albedo":
                                                            resource.AlbedoTexture = expression;
                                                            break;
                                                        case "normal":
                                                            resource.NormalTexture = expression;
                                                            break;
                                                        case "metallicity":
                                                            resource.MetallicityTexture = expression;
                                                            break;
                                                        case "roughness":
                                                            resource.RoughnessTexture = expression;
                                                            break;
                                                        case "emission":
                                                            resource.EmissionTexture = expression;
                                                            break;
                                                    }
                                                    break;
                                                }
                                            }
                                        }
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
            return typeof(TResource).IsAssignableFrom(typeof(PbrMaterialResource));
        }
    }
}