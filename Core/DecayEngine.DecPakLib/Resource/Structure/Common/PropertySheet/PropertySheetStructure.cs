using System.Collections.Generic;
using ProtoBuf;

namespace DecayEngine.DecPakLib.Resource.Structure.Common.PropertySheet
{
    [ProtoContract]
    public class PropertySheetStructure : IResource
    {
        [ProtoMember(1)]
        public List<IPropertySheetValue> PropertySheetValues { get; set; }
    }
}