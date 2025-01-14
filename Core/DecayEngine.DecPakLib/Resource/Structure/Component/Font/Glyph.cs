using System.Collections.Generic;
using ProtoBuf;

namespace DecayEngine.DecPakLib.Resource.Structure.Component.Font
{
    [ProtoContract]
    public class Glyph : IResource
    {
        [ProtoMember(1)]
        public string Character { get; set; }

        [ProtoMember(2)]
        public float UMin { get; set; }
        [ProtoMember(3)]
        public float VMin { get; set; }

        [ProtoMember(4)]
        public float UMax { get; set; }
        [ProtoMember(5)]
        public float VMax { get; set; }

        [ProtoMember(6)]
        public List<KerningTableEntry> KerningTable { get; set; }
    }
}