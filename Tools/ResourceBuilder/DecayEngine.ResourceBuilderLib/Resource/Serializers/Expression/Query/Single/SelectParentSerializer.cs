using System.Linq;
using System.Xml.Linq;
using DecayEngine.DecPakLib.Resource;
using DecayEngine.DecPakLib.Resource.Expression.Query;
using DecayEngine.DecPakLib.Resource.Expression.Query.Collection.Terminator;
using DecayEngine.DecPakLib.Resource.Expression.Query.Initiator;
using DecayEngine.DecPakLib.Resource.Expression.Query.Single;

namespace DecayEngine.ResourceBuilderLib.Resource.Serializers.Expression.Query.Single
{
    public class SelectParentSerializer : IResourceSerializer<SelectParentExpression>
    {
        public string Tag => "parent";

        IResource IResourceSerializer.Deserialize(XElement element)
        {
            return Deserialize(element);
        }

        public XElement Serialize(IResource resource)
        {
            if (!(resource is SelectParentExpression specificResource)) return null;
            return Serialize(specificResource);
        }

        public XElement Serialize(SelectParentExpression resource)
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

        public SelectParentExpression Deserialize(XElement element)
        {
            if (element.Name != Tag) return null;

            SelectParentExpression resource = new SelectParentExpression();

            XElement child = element.Elements().FirstOrDefault();
            if (child == null) return resource;

            IResourceSerializer childExpressionSerializer = ResourceSerializationController.GetSerializer(child.Name.ToString());
            if (childExpressionSerializer == null) return resource;
            if (!childExpressionSerializer.BuildsType<IQueryExpression>()) return resource;
            if (childExpressionSerializer.BuildsType<SelectActiveSceneExpression>()) return resource; // Combination does not make sense
            if (childExpressionSerializer.BuildsType<SelectThisExpression>()) return resource; // Combination does not make sense
            if (childExpressionSerializer.BuildsType<IQueryCollectionTerminatorExpression>()) return resource; // Combination does not make sense

            IQueryExpression childExpression = (IQueryExpression) childExpressionSerializer.Deserialize(child);
            resource.Next = childExpression;

            return resource;
        }

        public bool BuildsType<TResource>()
            where TResource : IResource
        {
            return typeof(TResource).IsAssignableFrom(typeof(SelectParentExpression));
        }
    }
}