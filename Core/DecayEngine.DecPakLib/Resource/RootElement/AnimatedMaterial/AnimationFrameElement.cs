using System.Collections.Generic;
using DecayEngine.DecPakLib.Resource.Structure.Math;
using ProtoBuf;

namespace DecayEngine.DecPakLib.Resource.RootElement.AnimatedMaterial
{
    [ProtoContract]
    public class AnimationFrameElement : IResource
    {
        [ProtoMember(2000)]
        public List<VertexStructure> Vertices { get; set; }
        [ProtoMember(2001)]
        public List<TriangleStructure> Triangles { get; set; }
    }
}