using DecayEngine.DecPakLib.Resource.Structure.Math;
using ProtoBuf;

namespace DecayEngine.DecPakLib.Resource.Structure.Transform
{
    [ProtoContract]
    public class TransformStructure : IResource
    {
        [ProtoMember(2000)]
        public Vector3Structure Position { get; set; }
        [ProtoMember(2001)]
        public Vector3Structure Rotation { get; set; }
        [ProtoMember(2002)]
        public Vector3Structure Scale { get; set; }

        [ProtoMember(2003)]
        public TransformMode Mode { get; set; }
    }
}