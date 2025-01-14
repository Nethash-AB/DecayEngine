using ProtoBuf;

namespace DecayEngine.DecPakLib.Resource.Structure.Math
{
    [ProtoContract]
    public class VertexStructure : IResource
    {
        [ProtoMember(2000)]
        public float X { get; set; }
        [ProtoMember(2001)]
        public float Y { get; set; }
        [ProtoMember(2002)]
        public float Z { get; set; }
        [ProtoMember(2003)]
        public float U { get; set; }
        [ProtoMember(2004)]
        public float V { get; set; }
    }
}