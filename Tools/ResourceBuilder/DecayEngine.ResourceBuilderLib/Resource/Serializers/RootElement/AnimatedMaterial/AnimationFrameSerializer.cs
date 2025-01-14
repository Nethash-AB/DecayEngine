using System.Collections.Generic;
using System.Xml.Linq;
using DecayEngine.DecPakLib.Resource;
using DecayEngine.DecPakLib.Resource.RootElement.AnimatedMaterial;
using DecayEngine.DecPakLib.Resource.Structure.Math;

namespace DecayEngine.ResourceBuilderLib.Resource.Serializers.RootElement.AnimatedMaterial
{
    public class AnimationFrameSerializer : IResourceSerializer<AnimationFrameElement>
    {
        public string Tag => "frame";

        IResource IResourceSerializer.Deserialize(XElement element)
        {
            return Deserialize(element);
        }

        public XElement Serialize(IResource resource)
        {
            if (!(resource is AnimationFrameElement specificResource)) return null;
            return Serialize(specificResource);
        }

        public XElement Serialize(AnimationFrameElement resource)
        {
            XElement element = new XElement(Tag);
            XElement verticesElement = new XElement("vertices");
            XElement trianglesElement = new XElement("triangles");

            IResourceSerializer<VertexStructure> vertexSerializer = ResourceSerializationController.GetSerializer<VertexStructure>();
            if (vertexSerializer != null)
            {
                foreach (VertexStructure vertex in resource.Vertices)
                {
                    XElement vertexElement = vertexSerializer.Serialize(vertex);
                    if (vertexElement == null) continue;

                    verticesElement.Add(vertexElement);
                }
            }

            IResourceSerializer<TriangleStructure> triangleSerializer = ResourceSerializationController.GetSerializer<TriangleStructure>();
            if (triangleSerializer != null)
            {
                foreach (TriangleStructure triangle in resource.Triangles)
                {
                    XElement triangleElement = triangleSerializer.Serialize(triangle);
                    if (triangleElement == null) continue;

                    trianglesElement.Add(triangleElement);
                }
            }

            if (verticesElement.HasElements)
            {
                element.Add(verticesElement);
            }

            if (trianglesElement.HasElements)
            {
                element.Add(trianglesElement);
            }

            return element;
        }

        public AnimationFrameElement Deserialize(XElement element)
        {
            if (element.Name != Tag) return null;

            AnimationFrameElement resource = new AnimationFrameElement
            {
                Vertices = new List<VertexStructure>(),
                Triangles = new List<TriangleStructure>()
            };

            XElement verticesElement = element.Element("vertices");
            if (verticesElement != null)
            {
                IResourceSerializer<VertexStructure> vertexSerializer = ResourceSerializationController.GetSerializer<VertexStructure>();
                if (vertexSerializer != null)
                {
                    foreach (XElement vertexElement in verticesElement.Elements())
                    {
                        VertexStructure vertex = vertexSerializer.Deserialize(vertexElement);
                        if (vertex == null) continue;

                        resource.Vertices.Add(vertex);
                    }
                }
            }

            XElement trianglesElement = element.Element("triangles");
            if (trianglesElement != null)
            {
                IResourceSerializer<TriangleStructure> triangleSerializer = ResourceSerializationController.GetSerializer<TriangleStructure>();
                if (triangleSerializer != null)
                {
                    foreach (XElement triangleElement in trianglesElement.Elements())
                    {
                        TriangleStructure triangle = triangleSerializer.Deserialize(triangleElement);
                        if (triangle == null) continue;

                        resource.Triangles.Add(triangle);
                    }
                }
            }

            return resource;
        }

        public bool BuildsType<TResource>()
            where TResource : IResource
        {
            return typeof(TResource).IsAssignableFrom(typeof(AnimationFrameElement));
        }
    }
}