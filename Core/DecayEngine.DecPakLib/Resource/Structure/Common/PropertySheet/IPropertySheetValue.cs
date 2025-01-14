using DecayEngine.DecPakLib.Resource.Structure.Common.PropertySheet.Values;
using ProtoBuf;

namespace DecayEngine.DecPakLib.Resource.Structure.Common.PropertySheet
{
    [ProtoContract]
    [ProtoInclude(2001, typeof(BoolPropertySheetValue))]
    [ProtoInclude(2002, typeof(NumericPropertySheetValue))]
    [ProtoInclude(2003, typeof(StringPropertySheetValue))]
    [ProtoInclude(2004, typeof(Vector2PropertySheetValue))]
    [ProtoInclude(2005, typeof(Vector3PropertySheetValue))]
    [ProtoInclude(2006, typeof(Vector4PropertySheetValue))]
    public interface IPropertySheetValue : IResource
    {
        [ProtoMember(1)]
        string Name { get; set; }
    }
}