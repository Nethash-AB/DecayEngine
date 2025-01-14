using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using DecayEngine.DecPakLib.Resource;
using DecayEngine.DecPakLib.Resource.Structure.Common.PropertySheet;
using DecayEngine.DecPakLib.Resource.Structure.Common.PropertySheet.Values;
using DecayEngine.DecPakLib.Resource.Structure.Math;

namespace DecayEngine.ResourceBuilderLib.Resource.Serializers.Structure.Common.PropertySheet
{
    public class PropertySheetSerializer : IResourceSerializer<PropertySheetStructure>
    {
        public string Tag => "propertySheet";

        IResource IResourceSerializer.Deserialize(XElement element)
        {
            return Deserialize(element);
        }

        public XElement Serialize(IResource resource)
        {
            if (!(resource is PropertySheetStructure specificResource)) return null;
            return Serialize(specificResource);
        }

        public XElement Serialize(PropertySheetStructure resource)
        {
            XElement element = new XElement(Tag);

            if (resource.PropertySheetValues == null) return null;

            foreach (IPropertySheetValue propertySheetValue in resource.PropertySheetValues)
            {
                XElement propertySheetValueElement = SerializePropertySheetValue(propertySheetValue);
                if (propertySheetValueElement != null)
                {
                    element.Add(propertySheetValueElement);
                }
            }

            return element;
        }

        public PropertySheetStructure Deserialize(XElement element)
        {
            if (element.Name != Tag) return null;

            PropertySheetStructure resource = new PropertySheetStructure
            {
                PropertySheetValues = new List<IPropertySheetValue>()
            };

            foreach (XElement propertySheetValueElement in element.Elements())
            {
                IPropertySheetValue propertySheetValue = DeserializePropertySheetValue(propertySheetValueElement);
                if (propertySheetValue == null) continue;

                if (resource.PropertySheetValues.All(kv => kv.Name != propertySheetValue.Name))
                {
                    resource.PropertySheetValues.Add(propertySheetValue);
                }
            }

            return resource;
        }

        public bool BuildsType<TResource>()
            where TResource : IResource
        {
            return typeof(TResource).IsAssignableFrom(typeof(PropertySheetStructure));
        }

        private static XElement SerializePropertySheetValue(IPropertySheetValue propertySheetValue)
        {
            XElement propertySheetValueElement = null;

            switch (propertySheetValue)
            {
                case NumericPropertySheetValue numericPropertySheetValue:
                    propertySheetValueElement = new XElement("number");
                    propertySheetValueElement.SetAttributeValue("name", numericPropertySheetValue.Name);
                    propertySheetValueElement.Value = numericPropertySheetValue.Value.ToString(CultureInfo.InvariantCulture);
                    break;
                case StringPropertySheetValue stringPropertySheetValue:
                    propertySheetValueElement = new XElement("string");
                    propertySheetValueElement.SetAttributeValue("name", stringPropertySheetValue.Name);
                    propertySheetValueElement.Value = stringPropertySheetValue.Value;
                    break;
                case BoolPropertySheetValue boolPropertySheetValue:
                    propertySheetValueElement = new XElement("bool");
                    propertySheetValueElement.SetAttributeValue("name", boolPropertySheetValue.Name);
                    propertySheetValueElement.Value = boolPropertySheetValue.Value.ToString();
                    break;
                case Vector2PropertySheetValue vector2PropertySheetValue:
                    propertySheetValueElement = new XElement("vector2");
                    propertySheetValueElement.SetAttributeValue("name", vector2PropertySheetValue.Name);
                    propertySheetValueElement.Value =
                        $"{vector2PropertySheetValue.Value.X},{vector2PropertySheetValue.Value.Y}";
                    break;
                case Vector3PropertySheetValue vector3PropertySheetValue:
                    propertySheetValueElement = new XElement("vector3");
                    propertySheetValueElement.SetAttributeValue("name", vector3PropertySheetValue.Name);
                    propertySheetValueElement.Value =
                        $"{vector3PropertySheetValue.Value.X},{vector3PropertySheetValue.Value.Y},{vector3PropertySheetValue.Value.Z}";
                    break;
                case Vector4PropertySheetValue vector4PropertySheetValue:
                    propertySheetValueElement = new XElement("vector4");
                    propertySheetValueElement.SetAttributeValue("name", vector4PropertySheetValue.Name);
                    propertySheetValueElement.Value =
                        $"{vector4PropertySheetValue.Value.X},{vector4PropertySheetValue.Value.Y}," +
                        $"{vector4PropertySheetValue.Value.Z},{vector4PropertySheetValue.Value.W}";
                    break;
            }

            return propertySheetValueElement;
        }

        private static IPropertySheetValue DeserializePropertySheetValue(XElement element)
        {
            XAttribute nameAttribute = element.Attribute("name");
            if (nameAttribute == null || string.IsNullOrEmpty(nameAttribute.Value) || string.IsNullOrEmpty(element.Value)) return null;

            IPropertySheetValue propertySheetValue = null;

            switch (element.Name.ToString())
            {
                case "number":
                    NumericPropertySheetValue numericPropertySheetValue = new NumericPropertySheetValue
                    {
                        Name = nameAttribute.Value
                    };
                    numericPropertySheetValue.Value = double.Parse(element.Value);

                    propertySheetValue = numericPropertySheetValue;
                    break;
                case "string":
                    StringPropertySheetValue stringPropertySheetValue = new StringPropertySheetValue
                    {
                        Name = nameAttribute.Value
                    };
                    stringPropertySheetValue.Value = element.Value;

                    propertySheetValue = stringPropertySheetValue;
                    break;
                case "bool":
                    BoolPropertySheetValue boolPropertySheetValue = new BoolPropertySheetValue
                    {
                        Name = nameAttribute.Value
                    };
                    boolPropertySheetValue.Value = bool.Parse(element.Value);

                    propertySheetValue = boolPropertySheetValue;
                    break;
                case "vector2":
                {
                    string[] vectorValues = element.Value.Split(',');
                    if (vectorValues.Length < 2) return null;

                    Vector2Structure vecValue = new Vector2Structure
                    {
                        X = float.Parse(vectorValues[0], NumberStyles.Any, CultureInfo.InvariantCulture),
                        Y = float.Parse(vectorValues[1], NumberStyles.Any, CultureInfo.InvariantCulture)
                    };

                    Vector2PropertySheetValue vector2PropertySheetValue = new Vector2PropertySheetValue
                    {
                        Name = nameAttribute.Value,
                        Value = vecValue
                    };
                    propertySheetValue = vector2PropertySheetValue;
                }
                    break;
                case "vector3":
                {
                    string[] vectorValues = element.Value.Split(',');
                    if (vectorValues.Length < 3) return null;

                    Vector3Structure vecValue = new Vector3Structure
                    {
                        X = float.Parse(vectorValues[0], NumberStyles.Any, CultureInfo.InvariantCulture),
                        Y = float.Parse(vectorValues[1], NumberStyles.Any, CultureInfo.InvariantCulture),
                        Z = float.Parse(vectorValues[2], NumberStyles.Any, CultureInfo.InvariantCulture)
                    };

                    Vector3PropertySheetValue vector3PropertySheetValue = new Vector3PropertySheetValue
                    {
                        Name = nameAttribute.Value,
                        Value = vecValue
                    };
                    propertySheetValue = vector3PropertySheetValue;
                }
                    break;
                case "vector4":
                {
                    string[] vectorValues = element.Value.Split(',');
                    if (vectorValues.Length < 4) return null;

                    Vector4Structure vecValue = new Vector4Structure
                    {
                        X = float.Parse(vectorValues[0], NumberStyles.Any, CultureInfo.InvariantCulture),
                        Y = float.Parse(vectorValues[1], NumberStyles.Any, CultureInfo.InvariantCulture),
                        Z = float.Parse(vectorValues[2], NumberStyles.Any, CultureInfo.InvariantCulture),
                        W = float.Parse(vectorValues[3], NumberStyles.Any, CultureInfo.InvariantCulture)
                    };

                    Vector4PropertySheetValue vector4PropertySheetValue = new Vector4PropertySheetValue
                    {
                        Name = nameAttribute.Value,
                        Value = vecValue
                    };
                    propertySheetValue = vector4PropertySheetValue;
                }
                    break;
            }

            return propertySheetValue;
        }
    }
}