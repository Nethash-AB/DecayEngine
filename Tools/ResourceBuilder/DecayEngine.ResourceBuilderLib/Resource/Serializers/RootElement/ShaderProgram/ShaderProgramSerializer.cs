using System.Linq;
using System.Xml.Linq;
using DecayEngine.DecPakLib.Resource;
using DecayEngine.DecPakLib.Resource.Expression.Property;
using DecayEngine.DecPakLib.Resource.RootElement.ShaderProgram;

namespace DecayEngine.ResourceBuilderLib.Resource.Serializers.RootElement.ShaderProgram
{
    public class ShaderProgramSerializer : IResourceSerializer<ShaderProgramResource>
    {
        public string Tag => "shaderProgram";

        IResource IResourceSerializer.Deserialize(XElement element)
        {
            return Deserialize(element);
        }

        public XElement Serialize(IResource resource)
        {
            if (!(resource is ShaderProgramResource specificResource)) return null;
            return Serialize(specificResource);
        }

        public XElement Serialize(ShaderProgramResource resource)
        {
            XElement element = new XElement(Tag);
            element.SetAttributeValue("id", resource.Id);

            XElement propertiesElement = new XElement("properties");

            XElement shadersElement = new XElement("shaders");

            IResourceSerializer serializer = ResourceSerializationController.GetSerializer(resource.VertexShader);
            if (serializer != null && serializer.BuildsType<IPropertyExpression>())
            {
                XElement vertexElement = new XElement("vertex");

                XElement expressionElement = serializer.Serialize(resource.VertexShader);
                if (expressionElement != null)
                {
                    vertexElement.Add(expressionElement);
                    shadersElement.Add(vertexElement);
                }
            }

            serializer = ResourceSerializationController.GetSerializer(resource.GeometryShader);
            if (serializer != null && serializer.BuildsType<IPropertyExpression>())
            {
                XElement geometryElement = new XElement("geometry");

                XElement expressionElement = serializer.Serialize(resource.GeometryShader);
                if (expressionElement != null)
                {
                    geometryElement.Add(expressionElement);
                    shadersElement.Add(geometryElement);
                }
            }

            serializer = ResourceSerializationController.GetSerializer(resource.FragmentShader);
            if (serializer != null && serializer.BuildsType<IPropertyExpression>())
            {
                XElement fragmentElement = new XElement("fragment");

                XElement expressionElement = serializer.Serialize(resource.FragmentShader);
                if (expressionElement != null)
                {
                    fragmentElement.Add(expressionElement);
                    shadersElement.Add(fragmentElement);
                }
            }

            if (shadersElement.HasElements)
            {
                propertiesElement.Add(shadersElement);
            }

            element.Add(propertiesElement);

            return element;
        }

        public ShaderProgramResource Deserialize(XElement element)
        {
            if (element.Name != Tag) return null;

            ShaderProgramResource resource = new ShaderProgramResource
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
                                case "shaders":
                                    foreach (XElement shader in property.Elements())
                                    {
                                        XElement expressionElement = shader.Elements().FirstOrDefault();
                                        if(expressionElement == null) continue;

                                        IResourceSerializer serializer = ResourceSerializationController.GetSerializer(expressionElement.Name.ToString());
                                        IPropertyExpression expression = (IPropertyExpression) serializer?.Deserialize(expressionElement);
                                        if (expression == null) continue;

                                        switch (shader.Name.ToString())
                                        {
                                            case "vertex":
                                                resource.VertexShader = expression;
                                                break;
                                            case "geometry":
                                                resource.GeometryShader = expression;
                                                break;
                                            case "fragment":
                                                resource.FragmentShader = expression;
                                                break;
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
            return typeof(TResource).IsAssignableFrom(typeof(ShaderProgramResource));
        }
    }
}