using ProtoBuf;

namespace DecayEngine.DecPakLib.Resource.Structure.Math
{
    [ProtoContract]
    public class TriangleStructure : IResource
    {
        [ProtoMember(2000)]
        public int Vertex1 { get; set; }
        [ProtoMember(2001)]
        public int Vertex2 { get; set; }
        [ProtoMember(2002)]
        public int Vertex3 { get; set; }
    }
}