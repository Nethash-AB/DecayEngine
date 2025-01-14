using System.Collections.Generic;
using System.Xml.Linq;
using DecayEngine.DecPakLib.Resource;
using DecayEngine.DecPakLib.Resource.Expression.Property;
using DecayEngine.DecPakLib.Resource.Expression.Statement;
using DecayEngine.DecPakLib.Resource.Expression.Statement.GameObject;
using DecayEngine.DecPakLib.Resource.Structure.Transform;

namespace DecayEngine.ResourceBuilderLib.Resource.Serializers.Expression.Statement.GameObject
{
    public class CreateGameObjectSerializer : IResourceSerializer<CreateGameObjectExpression>
    {
        public string Tag => "gameObject";

        IResource IResourceSerializer.Deserialize(XElement element)
        {
            return Deserialize(element);
        }

        public XElement Serialize(IResource resource)
        {
            if (!(resource is CreateGameObjectExpression specificResource)) return null;
            return Serialize(specificResource);
        }

        public XElement Serialize(CreateGameObjectExpression resource)
        {
            XElement element = new XElement(Tag);
            element.SetAttributeValue("name", resource.Name);
            element.SetAttributeValue("active", resource.Active);

            if (resource.Template != null)
            {
                IResourceSerializer serializer = ResourceSerializationController.GetSerializer(resource.Template);
                if (serializer != null && serializer.BuildsType<IPropertyExpression>())
                {
                    XElement propertyExpressionElement = serializer.Serialize(resource.Template);
                    if (propertyExpressionElement != null)
                    {
                        XElement tempateElement = new XElement("template");
                        tempateElement.Add(propertyExpressionElement);
                        element.Add(tempateElement);
                    }
                }
            }

            if (resource.Transform != null)
            {
                XElement transformElement = ResourceSerializationController.GetSerializer<TransformStructure>().Serialize(resource.Transform);
                if (transformElement != null)
                {
                    XElement propertiesElement = new XElement("properties");
                    propertiesElement.Add(transformElement);
                    element.Add(propertiesElement);
                }
            }

            if (resource.Children != null && resource.Children.Count > 0)
            {
                XElement childrenElement = new XElement("children");
                foreach (IStatementExpression child in resource.Children)
                {
                    IResourceSerializer serializer = ResourceSerializationController.GetSerializer(child);
                    if (serializer == null || !serializer.BuildsType<IStatementExpression>()) continue;

                    XElement childElement = serializer.Serialize(child);
                    if (childElement == null) continue;

                    childrenElement.Add(childElement);
                }

                if (childrenElement.HasElements)
                {
                    element.Add(childrenElement);
                }
            }

            return element;
        }

        public CreateGameObjectExpression Deserialize(XElement element)
        {
            if (element.Name != Tag) return null;

            CreateGameObjectExpression resource = new CreateGameObjectExpression
            {
                Name = element.Attribute("name")?.Value,
                Active = bool.Parse(element.Attribute("active")?.Value),
                Children = new List<IStatementExpression>()
            };

            foreach (XElement child in element.Elements())
            {
                switch (child.Name.ToString())
                {
                    case "template":
                        foreach (XElement template in child.Elements())
                        {
                            IResourceSerializer expressionSerializer = ResourceSerializationController.GetSerializer(template.Name.ToString());
                            if (expressionSerializer == null || !expressionSerializer.BuildsType<IPropertyExpression>()) continue;

                            IPropertyExpression expression = (IPropertyExpression) expressionSerializer.Deserialize(template);
                            if (expression == null) continue;

                            resource.Template = expression;
                        }
                        break;
                    case "properties":
                        foreach (XElement property in child.Elements())
                        {
                            switch (property.Name.ToString())
                            {
                                case "transform":
                                    IResourceSerializer<TransformStructure> transformSerializer = ResourceSerializationController.GetSerializer<TransformStructure>();
                                    if (transformSerializer == null) continue;

                                    resource.Transform = transformSerializer.Deserialize(property);
                                    break;
                            }
                        }
                        break;
                    case "children":
                        foreach (XElement expressionElement in child.Elements())
                        {
                            IResourceSerializer expressionSerializer = ResourceSerializationController.GetSerializer(expressionElement.Name.ToString());
                            if (expressionSerializer == null || !expressionSerializer.BuildsType<IStatementExpression>()) continue;

                            IStatementExpression expression = (IStatementExpression) expressionSerializer.Deserialize(expressionElement);
                            if (expression != null)
                            {
                                resource.Children.Add(expression);
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
            return typeof(TResource).IsAssignableFrom(typeof(CreateGameObjectExpression));
        }
    }
}