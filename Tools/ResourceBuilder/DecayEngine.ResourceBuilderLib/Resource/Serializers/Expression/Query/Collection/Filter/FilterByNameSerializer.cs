using System.Linq;
using System.Xml.Linq;
using DecayEngine.DecPakLib.Resource;
using DecayEngine.DecPakLib.Resource.Expression.Query;
using DecayEngine.DecPakLib.Resource.Expression.Query.Collection.Filter;

namespace DecayEngine.ResourceBuilderLib.Resource.Serializers.Expression.Query.Collection.Filter
{
    public class FilterByNameSerializer : IResourceSerializer<FilterByNameExpression>
    {
        public string Tag => "withName";

        IResource IResourceSerializer.Deserialize(XElement element)
        {
            return Deserialize(element);
        }

        public XElement Serialize(IResource resource)
        {
            if (!(resource is FilterByNameExpression specificResource)) return null;
            return Serialize(specificResource);
        }

        public XElement Serialize(FilterByNameExpression resource)
        {
            XElement element = new XElement(Tag);
            element.SetAttributeValue("name", resource.Name);

            IResourceSerializer serializer = ResourceSerializationController.GetSerializer(resource.Next);
            if (serializer == null || !serializer.BuildsType<IQueryExpression>()) return element;

            XElement childElement = serializer.Serialize(resource.Next);
            if (childElement != null)
            {
                element.Add(childElement);
            }

            return element;
        }

        public FilterByNameExpression Deserialize(XElement element)
        {
            if (element.Name != Tag) return null;

            FilterByNameExpression resource = new FilterByNameExpression
            {
                Name = element.Attribute("name")?.Value
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
            return typeof(TResource).IsAssignableFrom(typeof(FilterByNameExpression));
        }
    }
}