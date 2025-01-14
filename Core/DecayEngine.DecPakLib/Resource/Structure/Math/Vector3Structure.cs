using DecayEngine.DecPakLib.Math.Vector;
using ProtoBuf;

namespace DecayEngine.DecPakLib.Resource.Structure.Math
{
    [ProtoContract]
    public class Vector3Structure : IResource
    {
        [ProtoMember(2000)]
        public float X { get; set; }
        [ProtoMember(2001)]
        public float Y { get; set; }
        [ProtoMember(2002)]
        public float Z { get; set; }

        [ProtoMember(2003)]
        public bool WasNull { get; set; }

        public static implicit operator Vector3(Vector3Structure s)
        {
            return new Vector3(s.X, s.Y, s.Z);
        }
    }
}