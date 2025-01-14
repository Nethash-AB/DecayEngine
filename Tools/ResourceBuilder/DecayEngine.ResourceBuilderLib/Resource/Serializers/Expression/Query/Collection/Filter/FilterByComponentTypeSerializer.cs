using System;
using System.Linq;
using System.Xml.Linq;
using DecayEngine.DecPakLib.Resource;
using DecayEngine.DecPakLib.Resource.Expression.Query;
using DecayEngine.DecPakLib.Resource.Expression.Query.Collection.Filter;
using DecayEngine.DecPakLib.Resource.Structure.Component;

namespace DecayEngine.ResourceBuilderLib.Resource.Serializers.Expression.Query.Collection.Filter
{
    public class FilterByComponentTypeSerializer : IResourceSerializer<FilterByComponentTypeExpression>
    {
        public string Tag => "ofType";

        IResource IResourceSerializer.Deserialize(XElement element)
        {
            return Deserialize(element);
        }

        public XElement Serialize(IResource resource)
        {
            if (!(resource is FilterByComponentTypeExpression specificResource)) return null;
            return Serialize(specificResource);
        }

        public XElement Serialize(FilterByComponentTypeExpression resource)
        {
            XElement element = new XElement(Tag);

            char[] typeChars = resource.Type.ToString().ToCharArray();
            typeChars[0] = char.ToLowerInvariant(typeChars[0]);

            element.SetAttributeValue("type", new string(typeChars));

            IResourceSerializer serializer = ResourceSerializationController.GetSerializer(resource.Next);
            if (serializer == null || !serializer.BuildsType<IQueryExpression>()) return element;

            XElement childElement = serializer.Serialize(resource.Next);
            if (childElement != null)
            {
                element.Add(childElement);
            }

            return element;
        }

        public FilterByComponentTypeExpression Deserialize(XElement element)
        {
            if (element.Name != Tag) return null;

            FilterByComponentTypeExpression resource = new FilterByComponentTypeExpression
            {
                Type = (ComponentType) Enum.Parse(typeof(ComponentType), element.Attribute("type")?.Value, true)
            };

            XElement child = element.Elements().FirstOrDefault();
            if (child == null) return resource;

            IResourceSerializer childExpressionSerializer = ResourceSerializationController.GetSerializer(child.Name.ToString());
            if (childExpressionSerializer == null) return resource;
            if (!childExpressionSerializer.BuildsType<IQueryExpression>()) return resource;

            IQueryExpression childExpression = (IQueryExpression) childExpressionSerializer.Deserialize(child);
            resource.Next = childExpression;

            return resource;
        }

        public bool BuildsType<TResource>()
            where TResource : IResource
        {
            return typeof(TResource).IsAssignableFrom(typeof(FilterByComponentTypeExpression));
        }
    }
}