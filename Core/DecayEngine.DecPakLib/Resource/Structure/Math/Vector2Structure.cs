using DecayEngine.DecPakLib.Math.Vector;
using ProtoBuf;

namespace DecayEngine.DecPakLib.Resource.Structure.Math
{
    [ProtoContract]
    public class Vector2Structure : IResource
    {
        [ProtoMember(2000)]
        public float X { get; set; }
        [ProtoMember(2001)]
        public float Y { get; set; }

        [ProtoMember(2002)]
        public bool WasNull { get; set; }

        public static implicit operator Vector2(Vector2Structure s)
        {
            return new Vector2(s.X, s.Y);
        }
    }
}