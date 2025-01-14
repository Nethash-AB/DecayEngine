using System.Linq;
using System.Xml.Linq;
using DecayEngine.DecPakLib.Resource;
using DecayEngine.DecPakLib.Resource.Expression.Query.Initiator;
using DecayEngine.DecPakLib.Resource.Structure.Component.Script;

namespace DecayEngine.ResourceBuilderLib.Resource.Serializers.Structure.Component.Script
{
    public class ScriptInjectionSerializer : IResourceSerializer<ScriptInjection>
    {
        public string Tag => "injection";

        IResource IResourceSerializer.Deserialize(XElement element)
        {
            return Deserialize(element);
        }

        public XElement Serialize(IResource resource)
        {
            if (!(resource is ScriptInjection specificResource)) return null;
            return Serialize(specificResource);
        }

        public XElement Serialize(ScriptInjection resource)
        {
            XElement element = new XElement(Tag);
            element.SetAttributeValue("id", resource.Id);

            IResourceSerializer expressionSerializer = ResourceSerializationController.GetSerializer(resource.Expression);
            if (expressionSerializer == null || !expressionSerializer.BuildsType<IQueryInitiatorExpression>()) return null;

            XElement expressionElement = expressionSerializer.Serialize(resource.Expression);
            if (expressionElement == null) return null;

            element.Add(expressionElement);

            return element;
        }

        public ScriptInjection Deserialize(XElement element)
        {
            ScriptInjection resource = new ScriptInjection
            {
                Id = element.Attribute("id")?.Value
            };

            XElement firstChild = element.Elements().FirstOrDefault();
            if (firstChild == null) return null;

            IResourceSerializer expressionSerializer = ResourceSerializationController.GetSerializer(firstChild.Name.ToString());
            if (expressionSerializer == null || !expressionSerializer.BuildsType<IQueryInitiatorExpression>()) return null;

            resource.Expression = (IQueryInitiatorExpression) expressionSerializer.Deserialize(firstChild);

            if (resource.Id == null || resource.Expression == null) return null;
            return resource;
        }

        public bool BuildsType<TResource>()
            where TResource : IResource
        {
            return typeof(TResource).IsAssignableFrom(typeof(ScriptInjection));
        }
    }
}