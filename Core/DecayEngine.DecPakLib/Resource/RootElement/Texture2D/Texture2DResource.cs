using System.Collections.Generic;
using DecayEngine.DecPakLib.Platform;
using DecayEngine.DecPakLib.Pointer;
using ProtoBuf;

namespace DecayEngine.DecPakLib.Resource.RootElement.Texture2D
{
    [ProtoContract]
    public class Texture2DResource : IRootResource
    {
        [ProtoMember(1)]
        public string Id { get; set; }
        [ProtoMember(2)]
        public string MetaFilePath { get; set; }
        [ProtoMember(3)]
        public TargetPlatform TargetPlatforms { get; set; }

        [ProtoIgnore]
        public List<ByReference<DataPointer>> Pointers => new List<ByReference<DataPointer>>{() => ref Source};

        [ProtoMember(5, AsReference = true)]
        public DataPointer Source;
    }
}