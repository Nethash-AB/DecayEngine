using System.Linq;
using System.Xml.Linq;
using DecayEngine.DecPakLib.Pointer;
using DecayEngine.DecPakLib.Resource;
using DecayEngine.DecPakLib.Resource.Expression.Property;
using DecayEngine.DecPakLib.Resource.RootElement.Mesh;
using DecayEngine.DecPakLib.Resource.RootElement.Script;

namespace DecayEngine.ResourceBuilderLib.Resource.Serializers.RootElement.Mesh
{
    public class MeshSerializer : IResourceSerializer<MeshResource>
    {
        public string Tag => "mesh";

        IResource IResourceSerializer.Deserialize(XElement element)
        {
            return Deserialize(element);
        }

        public XElement Serialize(IResource resource)
        {
            if (!(resource is MeshResource specificResource)) return null;
            return Serialize(specificResource);
        }

        public XElement Serialize(MeshResource resource)
        {
            XElement element = new XElement(Tag);
            element.SetAttributeValue("id", resource.Id);

            XElement propertiesElement = new XElement("properties");

            XElement sourceElement = new XElement("source", resource.Source.SourcePath);
            propertiesElement.Add(sourceElement);

            element.Add(propertiesElement);

            return element;
        }

        public MeshResource Deserialize(XElement element)
        {
            if (element.Name != Tag) return null;

            MeshResource resource = new MeshResource
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
                                case "source":
                                {
                                    resource.Source = new DataPointer
                                    {
                                        SourcePath = property.Value
                                    };
                                    break;
                                }
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
            return typeof(TResource).IsAssignableFrom(typeof(ScriptResource));
        }
    }
}