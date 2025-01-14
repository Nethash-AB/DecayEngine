using System.Linq;
using System.Xml.Linq;
using DecayEngine.DecPakLib.Resource;
using DecayEngine.DecPakLib.Resource.Expression.Query;
using DecayEngine.DecPakLib.Resource.Expression.Query.Collection;
using DecayEngine.DecPakLib.Resource.Expression.Query.Collection.Filter;

namespace DecayEngine.ResourceBuilderLib.Resource.Serializers.Expression.Query.Collection
{
    public class SelectChildrenSerializer : IResourceSerializer<SelectChildrenExpression>
    {
        public string Tag => "children";

        IResource IResourceSerializer.Deserialize(XElement element)
        {
            return Deserialize(element);
        }

        public XElement Serialize(IResource resource)
        {
            if (!(resource is SelectChildrenExpression specificResource)) return null;
            return Serialize(specificResource);
        }

        public XElement Serialize(SelectChildrenExpression resource)
        {
            XElement element = new XElement(Tag);

            IResourceSerializer serializer = ResourceSerializationController.GetSerializer(resource.Next);
            if (serializer == null || !serializer.BuildsType<IQueryExpression>()) return element;

            XElement childElement = serializer.Serialize(resource.Next);
            if (childElement != null)
            {
                element.Add(childElement);
            }

            return element;
        }

        public SelectChildrenExpression Deserialize(XElement element)
        {
            if (element.Name != Tag) return null;

            SelectChildrenExpression resource = new SelectChildrenExpression();

            XElement child = element.Elements().FirstOrDefault();
            if (child == null) return resource;

            IResourceSerializer childExpressionSerializer = ResourceSerializationController.GetSerializer(child.Name.ToString());
            if (childExpressionSerializer == null) return resource;
            if (!childExpressionSerializer.BuildsType<IQueryExpression>()) return resource;
            if (childExpressionSerializer.BuildsType<FilterByComponentTypeExpression>()) return resource; // Cannot filter children by component type.

            IQueryExpression childExpression = (IQueryExpression) childExpressionSerializer.Deserialize(child);
            resource.Next = childExpression;

            return resource;
        }

        public bool BuildsType<TResource>()
            where TResource : IResource
        {
            return typeof(TResource).IsAssignableFrom(typeof(SelectChildrenExpression));
        }
    }
}