using DecayEngine.DecPakLib.Math.Vector;
using ProtoBuf;

namespace DecayEngine.DecPakLib.Resource.Structure.Math
{
    [ProtoContract]
    public class Vector4Structure : IResource
    {
        [ProtoMember(2000)]
        public float X { get; set; }
        [ProtoMember(2001)]
        public float Y { get; set; }
        [ProtoMember(2002)]
        public float Z { get; set; }
        [ProtoMember(2003)]
        public float W { get; set; }

        [ProtoMember(2004)]
        public bool WasNull { get; set; }

        public static implicit operator Vector4(Vector4Structure s)
        {
            return new Vector4(s.X, s.Y, s.Z, s.W);
        }
    }
}