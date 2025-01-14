using DecayEngine.DecPakLib.Pointer;
using ProtoBuf;

namespace DecayEngine.DecPakLib.Resource.RootElement.Shader
{
    [ProtoContract]
    public class ShaderFallback : IResource
    {
        [ProtoMember(1)]
        public ShaderLanguage Language { get; set; }

        [ProtoMember(2)]
        public DataPointer Source
        {
            get => _fallbackPointer;
            set => _fallbackPointer = value;
        }

        [ProtoIgnore]
        public ByReference<DataPointer> SourceByReference => () => ref _fallbackPointer;

        [ProtoIgnore]
        private DataPointer _fallbackPointer;
    }
}