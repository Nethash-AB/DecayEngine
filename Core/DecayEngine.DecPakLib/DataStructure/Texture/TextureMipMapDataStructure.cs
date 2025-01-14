using ProtoBuf;

namespace DecayEngine.DecPakLib.DataStructure.Texture
{
    [ProtoContract]
    public class TextureMipMapDataStructure
    {
        [ProtoMember(1)]
        public int Width { get; set; }
        [ProtoMember(2)]
        public int Height { get; set; }
        [ProtoMember(3)]
        public int RowPitch { get; set; }
        [ProtoMember(4)]
        public byte[] PixelData { get; set; }
    }
}