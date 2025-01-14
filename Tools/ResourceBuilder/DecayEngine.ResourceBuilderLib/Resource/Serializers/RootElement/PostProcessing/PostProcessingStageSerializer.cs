using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using DecayEngine.DecPakLib.Resource;
using DecayEngine.DecPakLib.Resource.Expression.Property;
using DecayEngine.DecPakLib.Resource.RootElement.PostProcessing;
using DecayEngine.DecPakLib.Resource.Structure.Common.PropertySheet;

namespace DecayEngine.ResourceBuilderLib.Resource.Serializers.RootElement.PostProcessing
{
    public class PostProcessingStageSerializer : IResourceSerializer<PostProcessingStage>
    {
        public string Tag => "postProcessingStage";

        IResource IResourceSerializer.Deserialize(XElement element)
        {
            return Deserialize(element);
        }

        public XElement Serialize(IResource resource)
        {
            if (!(resource is PostProcessingStage specificResource)) return null;
            return Serialize(specificResource);
        }

        public XElement Serialize(PostProcessingStage resource)
        {
            XElement element = new XElement(Tag);
            element.SetAttributeValue("name", resource.Name);

            XElement propertiesElement = new XElement("properties");

            IResourceSerializer shaderProgramSerializer = ResourceSerializationController.GetSerializer(resource.ShaderProgram);
            if (shaderProgramSerializer != null && shaderProgramSerializer.BuildsType<IPropertyExpression>())
            {
                XElement shaderProgramElement = new XElement("shaderProgram");

                XElement expressionElement = shaderProgramSerializer.Serialize(resource.ShaderProgram);
                if (expressionElement != null)
                {
                    shaderProgramElement.Add(expressionElement);
                    propertiesElement.Add(shaderProgramElement);
                }
            }

            if (resource.Kernel != null && resource.Kernel.Length == 9)
            {
                XElement kernelElement = new XElement("kernel")
                {
                    Value = $"{resource.Kernel[0]},{resource.Kernel[1]},{resource.Kernel[2]}," +
                            $"{resource.Kernel[3]},{resource.Kernel[4]},{resource.Kernel[5]}," +
                            $"{resource.Kernel[6]},{resource.Kernel[7]},{resource.Kernel[8]}"
                };
                propertiesElement.Add(kernelElement);
            }

            IResourceSerializer<PropertySheetStructure> propertySheetSerializer = ResourceSerializationController.GetSerializer<PropertySheetStructure>();
            XElement propertySheetElement = propertySheetSerializer?.Serialize(resource.Properties);

            if (propertySheetElement != null)
            {
                propertiesElement.Add(propertySheetElement);
            }

            if (propertiesElement.HasElements)
            {
                element.Add(propertiesElement);
            }

            return element;
        }

        public PostProcessingStage Deserialize(XElement element)
        {
            if (element.Name != Tag) return null;

            PostProcessingStage resource = new PostProcessingStage
            {
                Name = element.Attribute("name")?.Value
            };

            XElement shaderProgramElement = element.Element("shaderProgram");
            XElement expressionElement = shaderProgramElement?.Elements().FirstOrDefault();
            if (expressionElement != null)
            {
                IResourceSerializer serializer = ResourceSerializationController.GetSerializer(expressionElement.Name.ToString());
                if (serializer != null && serializer.BuildsType<IPropertyExpression>())
                {
                    resource.ShaderProgram = (IPropertyExpression) serializer.Deserialize(expressionElement);
                }
            }

            XElement kernelElement = element.Element("kernel");
            if (kernelElement != null && !string.IsNullOrEmpty(kernelElement.Value))
            {
                string[] kernelStringValues = kernelElement.Value.Split(',');
                if (kernelStringValues.Length == 9)
                {
                    float[] kernelValues = new float[9];
                    for (int i = 0; i < 9; i++)
                    {
                        kernelValues[i] = float.Parse(kernelStringValues[i], NumberStyles.Any, CultureInfo.InvariantCulture);
                    }

                    resource.Kernel = kernelValues;
                }
            }

            XElement propertySheetElement = element.Element("propertySheet");
            if (propertySheetElement != null)
            {
                IResourceSerializer<PropertySheetStructure> propertySheetSerializer = ResourceSerializationController.GetSerializer<PropertySheetStructure>();
                if (propertySheetSerializer != null)
                {
                    resource.Properties = propertySheetSerializer.Deserialize(propertySheetElement);
                }
            }

            if (resource.Properties == null)
            {
                resource.Properties = new PropertySheetStructure();
            }

            return resource;
        }

        public bool BuildsType<TResource>()
            where TResource : IResource
        {
            return typeof(TResource).IsAssignableFrom(typeof(PostProcessingStage));
        }
    }
}