using ProtoBuf;

namespace DecayEngine.DecPakLib.Resource.Structure.Common.PropertySheet.Values
{
    [ProtoContract]
    public class StringPropertySheetValue : IPropertySheetValue
    {
        public string Name { get; set; }
        [ProtoMember(2)]
        public string Value { get; set; }
    }
}