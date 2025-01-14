using ProtoBuf;

namespace DecayEngine.DecPakLib.DataStructure.Texture
{
    [ProtoContract]
    public enum TextureDataFormat
    {
        Unknown,
        Rgba,
        Rgb5,
        Rgb5A1,
        Rgb8,
        Dxt1,
        Dxt3,
        Dxt5
    }
}