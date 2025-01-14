using DecayEngine.DecPakLib.Resource.Structure.Math;
using ProtoBuf;

namespace DecayEngine.DecPakLib.Resource.Structure.Common.PropertySheet.Values
{
    [ProtoContract]
    public class Vector3PropertySheetValue : IPropertySheetValue
    {
        public string Name { get; set; }
        [ProtoMember(2)]
        public Vector3Structure Value { get; set; }
    }
}