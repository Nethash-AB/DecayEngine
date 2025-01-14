using System;
using System.Xml.Linq;
using DecayEngine.DecPakLib.Resource;
using DecayEngine.DecPakLib.Resource.Structure.Math;
using DecayEngine.DecPakLib.Resource.Structure.Transform;

namespace DecayEngine.ResourceBuilderLib.Resource.Serializers.Structure.Transform
{
    public class TransformSerializer : IResourceSerializer<TransformStructure>
    {
        public string Tag => "transform";

        IResource IResourceSerializer.Deserialize(XElement element)
        {
            return Deserialize(element);
        }

        public XElement Serialize(IResource resource)
        {
            if (!(resource is TransformStructure specificResource)) return null;
            return Serialize(specificResource);
        }

        public XElement Serialize(TransformStructure resource)
        {
            XElement element = new XElement(Tag);

            IResourceSerializer<Vector3Structure> vectorSerializer = ResourceSerializationController.GetSerializer<Vector3Structure>();

            if (!resource.Position.WasNull)
            {
                XElement positionElement = vectorSerializer.Serialize(resource.Position);
                if (positionElement != null)
                {
                    positionElement.Name = "position";
                    element.Add(positionElement);
                }
            }

            if (!resource.Rotation.WasNull)
            {
                XElement rotationElement = vectorSerializer.Serialize(resource.Rotation);
                if (rotationElement != null)
                {
                    rotationElement.Name = "rotation";
                    element.Add(rotationElement);
                }
            }

            if (!resource.Scale.WasNull)
            {
                XElement scaleElement = vectorSerializer.Serialize(resource.Scale);
                if (scaleElement != null)
                {
                    scaleElement.Name = "scale";
                    element.Add(scaleElement);
                }
            }

            if (resource.Mode != TransformMode.NotSet)
            {
                element.SetAttributeValue("mode", resource.Mode.ToString().ToLower());
            }

            return element;
        }

        public TransformStructure Deserialize(XElement element)
        {
            if (element.Name != Tag) return null;

            TransformStructure resource = new TransformStructure
            {
                Mode = TransformMode.NotSet
            };

            XAttribute mode = element.Attribute("mode");
            if (mode != null)
            {
                resource.Mode = (TransformMode) Enum.Parse(typeof(TransformMode), mode.Value, true);
            }

            foreach (XElement child in element.Elements())
            {
                switch (child.Name.ToString())
                {
                    case "position":
                        resource.Position = ResourceSerializationController.GetSerializer<Vector3Structure>()?.Deserialize(child);
                        break;
                    case "rotation":
                        resource.Rotation = ResourceSerializationController.GetSerializer<Vector3Structure>()?.Deserialize(child);
                        break;
                    case "scale":
                        resource.Scale = ResourceSerializationController.GetSerializer<Vector3Structure>()?.Deserialize(child);
                        break;
                }
            }

            if (resource.Position == null)
            {
                resource.Position = new Vector3Structure
                {
                    X = 0,
                    Y = 0,
                    Z = 0,
                    WasNull = true
                };
            }

            if (resource.Rotation == null)
            {
                resource.Rotation = new Vector3Structure
                {
                    X = 0,
                    Y = 0,
                    Z = 0,
                    WasNull = true
                };
            }

            if (resource.Scale == null)
            {
                resource.Scale = new Vector3Structure
                {
                    X = 1,
                    Y = 1,
                    Z = 1,
                    WasNull = true
                };
            }

            return resource;
        }

        public bool BuildsType<TResource>()
            where TResource : IResource
        {
            return typeof(TResource).IsAssignableFrom(typeof(TransformStructure));
        }
    }
}