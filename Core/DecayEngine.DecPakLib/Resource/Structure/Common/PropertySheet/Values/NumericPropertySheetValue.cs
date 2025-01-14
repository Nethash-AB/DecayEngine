using ProtoBuf;

namespace DecayEngine.DecPakLib.Resource.Structure.Common.PropertySheet.Values
{
    [ProtoContract]
    public class NumericPropertySheetValue : IPropertySheetValue
    {
        public string Name { get; set; }
        [ProtoMember(2)]
        public double Value { get; set; }
    }
}