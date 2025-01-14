using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml.Linq;
using DecayEngine.DecPakLib.Resource;
using DecayEngine.DecPakLib.Resource.RootElement.Collider;
using DecayEngine.DecPakLib.Resource.Structure.Math;

namespace DecayEngine.ResourceBuilderLib.Resource.Serializers.RootElement.Collider
{
    public class Collider2DSerializer : IResourceSerializer<Collider2DResource>
    {
        public string Tag => "collider2d";

        IResource IResourceSerializer.Deserialize(XElement element)
        {
            return Deserialize(element);
        }

        public XElement Serialize(IResource resource)
        {
            if (!(resource is Collider2DResource specificResource)) return null;
            return Serialize(specificResource);
        }

        public XElement Serialize(Collider2DResource resource)
        {
            XElement element = new XElement(Tag);
            element.SetAttributeValue("id", resource.Id);

            XElement propertiesElement = new XElement("properties");

            XElement massElement = new XElement("mass", resource.Mass);
            propertiesElement.Add(massElement);

            XElement isTriggerElement = new XElement("isTrigger", resource.IsTrigger);
            propertiesElement.Add(isTriggerElement);

            XElement isStaticElement = new XElement("isStatic", resource.IsStatic);
            propertiesElement.Add(isStaticElement);

            XElement isKinematicElement = new XElement("isKinematic", resource.IsKinematic);
            propertiesElement.Add(isKinematicElement);

            XElement layerElement = new XElement("collisionMask",
                Convert.ToString(resource.CollisionMask, 2).PadLeft(32, '0'));
            propertiesElement.Add(layerElement);

            XElement marginElement = new XElement("margin", resource.Margin);
            propertiesElement.Add(marginElement);

            XElement frictionElement = new XElement("friction");

            XElement frictionLinearElement = new XElement("linear", resource.FrictionLinear);
            frictionElement.Add(frictionLinearElement);

            XElement frictionSpinningElement = new XElement("spinning", resource.FrictionSpinning);
            frictionElement.Add(frictionSpinningElement);

            XElement frictionRollingElement = new XElement("rolling", resource.FrictionRolling);
            frictionElement.Add(frictionRollingElement);

            propertiesElement.Add(frictionElement);

            if (resource.AnisotropicFriction != null)
            {
                IResourceSerializer<Vector3Structure> vector3Factory = ResourceSerializationController.GetSerializer<Vector3Structure>();
                if (vector3Factory != null)
                {
                    XElement anisotropicFrictionElement = vector3Factory.Serialize(resource.AnisotropicFriction);
                    anisotropicFrictionElement.Name = "anisotropicFriction";
                    propertiesElement.Add(anisotropicFrictionElement);
                }
            }

            XElement dampingElement = new XElement("damping");

            XElement dampingLinearElement = new XElement("linear", resource.DampingLinear);
            dampingElement.Add(dampingLinearElement);

            XElement dampingAngularElement = new XElement("angular", resource.DampingAngular);
            dampingElement.Add(dampingAngularElement);

            propertiesElement.Add(dampingElement);

            XElement sleepingTheresholdElement = new XElement("sleepingThereshold");

            XElement sleepingTheresholdLinearElement = new XElement("linear", resource.SleepingTheresholdLinear);
            sleepingTheresholdElement.Add(sleepingTheresholdLinearElement);

            XElement sleepingTheresholdAngularElement = new XElement("angular", resource.SleepingTheresholdAngular);
            sleepingTheresholdElement.Add(sleepingTheresholdAngularElement);

            propertiesElement.Add(sleepingTheresholdElement);

            XElement restitutionElement = new XElement("restitution", resource.Restitution);
            propertiesElement.Add(restitutionElement);

            XElement hullElement = new XElement("hull");
            IResourceSerializer<VertexStructure> vertexSerializer = ResourceSerializationController.GetSerializer<VertexStructure>();
            if (vertexSerializer != null)
            {
                foreach (VertexStructure hullVertex in resource.HullVertices)
                {
                    XElement vertexElement = vertexSerializer.Serialize(hullVertex);
                    if (vertexElement == null) continue;

                    hullElement.Add(vertexElement);
                }
            }

            if (hullElement.HasElements)
            {
                propertiesElement.Add(hullElement);
            }

            if (propertiesElement.HasElements)
            {
                element.Add(propertiesElement);
            }

            return element;
        }

        public Collider2DResource Deserialize(XElement element)
        {
            if (element.Name != Tag) return null;

            Collider2DResource resource = new Collider2DResource
            {
                Id = element.Attribute("id")?.Value,
                MetaFilePath = element.Attribute("filePath")?.Value,
                HullVertices = new List<VertexStructure>()
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
                                case "mass":
                                    resource.Mass = float.Parse(property.Value, NumberStyles.Any, CultureInfo.InvariantCulture);
                                    break;
                                case "isTrigger":
                                    resource.IsTrigger = bool.Parse(property.Value);
                                    break;
                                case "isStatic":
                                    resource.IsStatic = bool.Parse(property.Value);
                                    break;
                                case "isKinematic":
                                    resource.IsKinematic = bool.Parse(property.Value);
                                    break;
                                case "collisionMask":
                                    resource.CollisionMask = Convert.ToInt32(property.Value.PadLeft(32, '0'), 2);
                                    break;
                                case "margin":
                                    resource.Margin = float.Parse(property.Value, NumberStyles.Any, CultureInfo.InvariantCulture);
                                    break;
                                case "friction":
                                    foreach (XElement friction in property.Elements())
                                    {
                                        switch (friction.Name.ToString())
                                        {
                                            case "linear":
                                                resource.FrictionLinear = float.Parse(friction.Value, NumberStyles.Any, CultureInfo.InvariantCulture);
                                                break;
                                            case "spinning":
                                                resource.FrictionSpinning = float.Parse(friction.Value, NumberStyles.Any, CultureInfo.InvariantCulture);
                                                break;
                                            case "rolling":
                                                resource.FrictionRolling = float.Parse(friction.Value, NumberStyles.Any, CultureInfo.InvariantCulture);
                                                break;
                                        }
                                    }
                                    break;
                                case "anisotropicFriction":
                                    IResourceSerializer<Vector3Structure> vector3Serializer = ResourceSerializationController.GetSerializer<Vector3Structure>();

                                    Vector3Structure vector3 = vector3Serializer?.Deserialize(property);
                                    if (vector3 == null) continue;

                                    resource.AnisotropicFriction = vector3;
                                    break;
                                case "damping":
                                    foreach (XElement damping in property.Elements())
                                    {
                                        switch (damping.Name.ToString())
                                        {
                                            case "linear":
                                                resource.DampingLinear = float.Parse(damping.Value, NumberStyles.Any, CultureInfo.InvariantCulture);
                                                break;
                                            case "angular":
                                                resource.DampingAngular = float.Parse(damping.Value, NumberStyles.Any, CultureInfo.InvariantCulture);
                                                break;
                                        }
                                    }
                                    break;
                                case "sleepingThereshold":
                                    foreach (XElement sleepingThereshold in property.Elements())
                                    {
                                        switch (sleepingThereshold.Name.ToString())
                                        {
                                            case "linear":
                                                resource.SleepingTheresholdLinear = float.Parse(sleepingThereshold.Value,
                                                    NumberStyles.Any, CultureInfo.InvariantCulture);
                                                break;
                                            case "angular":
                                                resource.SleepingTheresholdAngular = float.Parse(sleepingThereshold.Value,
                                                    NumberStyles.Any, CultureInfo.InvariantCulture);
                                                break;
                                        }
                                    }
                                    break;
                                case "restitution":
                                    resource.Restitution = float.Parse(property.Value, NumberStyles.Any, CultureInfo.InvariantCulture);
                                    break;
                                case "hull":
                                    IResourceSerializer<VertexStructure> vertexSerializer = ResourceSerializationController.GetSerializer<VertexStructure>();
                                    if (vertexSerializer == null) continue;

                                    foreach (XElement vertexElement in property.Elements())
                                    {
                                        VertexStructure vertex = vertexSerializer.Deserialize(vertexElement);
                                        if (vertex == null) continue;

                                        resource.HullVertices.Add(vertex);
                                    }
                                    break;
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
            return typeof(TResource).IsAssignableFrom(typeof(Collider2DResource));
        }
    }
}