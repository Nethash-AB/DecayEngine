using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using DecayEngine.DecPakLib.Resource;
using DecayEngine.DecPakLib.Resource.Expression.Property;
using DecayEngine.DecPakLib.Resource.RootElement.AnimatedMaterial;

namespace DecayEngine.ResourceBuilderLib.Resource.Serializers.RootElement.AnimatedMaterial
{
    public class AnimatedMaterialSerializer : IResourceSerializer<AnimatedMaterialResource>
    {
        public string Tag => "animatedMaterial";

        IResource IResourceSerializer.Deserialize(XElement element)
        {
            return Deserialize(element);
        }

        public XElement Serialize(IResource resource)
        {
            if (!(resource is AnimatedMaterialResource specificResource)) return null;
            return Serialize(specificResource);
        }

        public XElement Serialize(AnimatedMaterialResource resource)
        {
            XElement element = new XElement(Tag);
            element.SetAttributeValue("id", resource.Id);

            XElement propertiesElement = new XElement("properties");
            XElement texturesElement = new XElement("textures");
            XElement animationFramesElement = new XElement("animationFrames");

            IResourceSerializer serializer = ResourceSerializationController.GetSerializer(resource.DiffuseTexture);
            if (serializer != null && serializer.BuildsType<IPropertyExpression>())
            {
                XElement diffuseElement = new XElement("diffuse");

                XElement expressionElement = serializer.Serialize(resource.DiffuseTexture);
                if (expressionElement != null)
                {
                    diffuseElement.Add(expressionElement);
                    texturesElement.Add(diffuseElement);
                }
            }

            serializer = ResourceSerializationController.GetSerializer(resource.NormalTexture);
            if (serializer != null && serializer.BuildsType<IPropertyExpression>())
            {
                XElement normalElement = new XElement("normal");

                XElement expressionElement = serializer.Serialize(resource.NormalTexture);
                if (expressionElement != null)
                {
                    normalElement.Add(expressionElement);
                    texturesElement.Add(normalElement);
                }
            }

            IResourceSerializer<AnimationFrameElement> animationFrameSerializer = ResourceSerializationController.GetSerializer<AnimationFrameElement>();
            if (animationFrameSerializer != null)
            {
                foreach (AnimationFrameElement animationFrame in resource.AnimationFrames)
                {
                    XElement animationFrameElement = animationFrameSerializer.Serialize(animationFrame);
                    if (animationFrameElement == null) continue;

                    animationFramesElement.Add(animationFrameElement);
                }
            }

            if (texturesElement.HasElements)
            {
                propertiesElement.Add(texturesElement);
            }

            if (animationFramesElement.HasElements)
            {
                propertiesElement.Add(animationFramesElement);
            }

            if (propertiesElement.HasElements)
            {
                element.Add(propertiesElement);
            }

            return element;
        }

        public AnimatedMaterialResource Deserialize(XElement element)
        {
            if (element.Name != Tag) return null;

            AnimatedMaterialResource resource = new AnimatedMaterialResource
            {
                Id = element.Attribute("id")?.Value,
                MetaFilePath = element.Attribute("filePath")?.Value,
                AnimationFrames = new List<AnimationFrameElement>()
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
                                        XElement expressionElement = texture.Elements().FirstOrDefault();
                                        if(expressionElement == null) continue;

                                        IResourceSerializer serializer = ResourceSerializationController.GetSerializer(expressionElement.Name.ToString());
                                        if (serializer == null || !serializer.BuildsType<IPropertyExpression>()) continue;

                                        IPropertyExpression expression = (IPropertyExpression) serializer.Deserialize(expressionElement);
                                        if (expression == null) continue;

                                        switch (texture.Name.ToString())
                                        {
                                            case "diffuse":
                                                resource.DiffuseTexture = expression;
                                                break;
                                            case "normal":
                                                resource.NormalTexture = expression;
                                                break;
                                        }
                                    }
                                    break;
                                case "animationFrames":
                                    IResourceSerializer<AnimationFrameElement> animationFrameSerializer =
                                        ResourceSerializationController.GetSerializer<AnimationFrameElement>();
                                    if (animationFrameSerializer == null) continue;

                                    foreach (XElement animationFrameElement in property.Elements())
                                    {
                                        AnimationFrameElement animationFrame = animationFrameSerializer.Deserialize(animationFrameElement);
                                        if (animationFrame == null) continue;

                                        resource.AnimationFrames.Add(animationFrame);
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
            return typeof(TResource).IsAssignableFrom(typeof(AnimatedMaterialResource));
        }
    }
}