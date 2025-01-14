using ProtoBuf;

namespace DecayEngine.DecPakLib.Resource.Structure.Component.Font
{
    [ProtoContract]
    public class KerningTableEntry : IResource
    {
        [ProtoMember(1)]
        public string Character { get; set; }

        [ProtoMember(2)]
        public float Kerning { get; set; }

        public KerningTableEntry()
        {
        }

        public KerningTableEntry(string character, float kerning)
        {
            Character = character;
            Kerning = kerning;
        }
    }
}