using System.Collections.Generic;
using System.Xml.Linq;
using DecayEngine.DecPakLib.Resource;
using DecayEngine.DecPakLib.Resource.Expression.Statement;
using DecayEngine.DecPakLib.Resource.RootElement.Prefab;

namespace DecayEngine.ResourceBuilderLib.Resource.Serializers.RootElement.Prefab
{
    public class PrefabSerializer : IResourceSerializer<PrefabResource>
    {
        public string Tag => "prefab";

        IResource IResourceSerializer.Deserialize(XElement element)
        {
            return Deserialize(element);
        }

        public XElement Serialize(IResource resource)
        {
            if (!(resource is PrefabResource specificResource)) return null;
            return Serialize(specificResource);
        }

        public XElement Serialize(PrefabResource resource)
        {
            XElement element = new XElement(Tag);
            element.SetAttributeValue("id", resource.Id);

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

            return element;
        }

        public PrefabResource Deserialize(XElement element)
        {
            if (element.Name != Tag) return null;

            PrefabResource resource = new PrefabResource
            {
                Id = element.Attribute("id")?.Value,
                MetaFilePath = element.Attribute("filePath")?.Value,
                Children = new List<IStatementExpression>()
            };

            foreach (XElement child in element.Elements())
            {
                switch (child.Name.ToString())
                {
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
            return typeof(TResource).IsAssignableFrom(typeof(PrefabResource));
        }
    }
}