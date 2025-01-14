using DecayEngine.DecPakLib.Pointer;
using ProtoBuf;

namespace DecayEngine.DecPakLib.Resource.RootElement.Font
{
    [ProtoContract]
    public class FontMipMap : IResource
    {
        [ProtoMember(1)]
        public DataPointer Texture
        {
            get => _texturePointer;
            set => _texturePointer = value;
        }

        [ProtoMember(2)]
        public int Size { get; set; }

        [ProtoIgnore]
        public ByReference<DataPointer> TextureByReference => () => ref _texturePointer;

        [ProtoIgnore]
        private DataPointer _texturePointer;
    }
}