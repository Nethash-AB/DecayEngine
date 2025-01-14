using System.Collections.Generic;
using DecayEngine.DecPakLib.Platform;
using DecayEngine.DecPakLib.Pointer;
using ProtoBuf;

namespace DecayEngine.DecPakLib.Resource.RootElement.Shader
{
    [ProtoContract]
    public class ShaderResource : IRootResource
    {
        [ProtoMember(1)]
        public string Id { get; set; }
        [ProtoMember(2)]
        public string MetaFilePath { get; set; }
        [ProtoMember(3)]
        public TargetPlatform TargetPlatforms { get; set; }

        [ProtoIgnore]
        public List<ByReference<DataPointer>> Pointers => new List<ByReference<DataPointer>>{() => ref Source};

        [ProtoMember(4)]
        public ShaderLanguage Language { get; set; }
        [ProtoMember(5)]
        public List<ShaderLanguage> FallbackTargets { get; set; }
        [ProtoMember(6)]
        public bool Compile { get; set; }
        [ProtoMember(7)]
        public ShaderType Type { get; set; }

        [ProtoMember(8, AsReference = true)]
        public DataPointer Source;
        [ProtoMember(9, AsReference = true)]
        public List<ShaderFallback> Fallbacks;
    }
}