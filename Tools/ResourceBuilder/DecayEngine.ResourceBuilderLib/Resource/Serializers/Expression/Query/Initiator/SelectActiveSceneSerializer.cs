using System.Linq;
using System.Xml.Linq;
using DecayEngine.DecPakLib.Resource;
using DecayEngine.DecPakLib.Resource.Expression.Query;
using DecayEngine.DecPakLib.Resource.Expression.Query.Collection.Terminator;
using DecayEngine.DecPakLib.Resource.Expression.Query.Initiator;

namespace DecayEngine.ResourceBuilderLib.Resource.Serializers.Expression.Query.Initiator
{
    public class SelectActiveSceneSerializer : IResourceSerializer<SelectActiveSceneExpression>
    {
        public string Tag => "activeScene";

        IResource IResourceSerializer.Deserialize(XElement element)
        {
            return Deserialize(element);
        }

        public XElement Serialize(IResource resource)
        {
            if (!(resource is SelectActiveSceneExpression specificResource)) return null;
            return Serialize(specificResource);
        }

        public XElement Serialize(SelectActiveSceneExpression resource)
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

        public SelectActiveSceneExpression Deserialize(XElement element)
        {
            if (element.Name != Tag) return null;

            SelectActiveSceneExpression resource = new SelectActiveSceneExpression();

            XElement child = element.Elements().FirstOrDefault();
            if (child == null) return resource;

            IResourceSerializer childExpressionSerializer = ResourceSerializationController.GetSerializer(child.Name.ToString());
            if (childExpressionSerializer == null) return resource;
            if (!childExpressionSerializer.BuildsType<IQueryExpression>()) return resource;
            if (childExpressionSerializer.BuildsType<SelectThisExpression>()) return resource; // Combination does not make sense
            if (childExpressionSerializer.BuildsType<SelectRootExpression>()) return resource; // Combination does not make sense
            if (childExpressionSerializer.BuildsType<SelectGlobalExpression>()) return resource; // Combination does not make sense
            if (childExpressionSerializer.BuildsType<IQueryCollectionTerminatorExpression>()) return resource; // Combination does not make sense

            IQueryExpression childExpression = (IQueryExpression) childExpressionSerializer.Deserialize(child);
            resource.Next = childExpression;

            return resource;
        }

        public bool BuildsType<TResource>()
            where TResource : IResource
        {
            return typeof(TResource).IsAssignableFrom(typeof(SelectActiveSceneExpression));
        }
    }
}