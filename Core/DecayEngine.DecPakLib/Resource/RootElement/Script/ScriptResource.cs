using System.Collections.Generic;
using DecayEngine.DecPakLib.Platform;
using DecayEngine.DecPakLib.Pointer;
using ProtoBuf;

namespace DecayEngine.DecPakLib.Resource.RootElement.Script
{
    [ProtoContract]
    public class ScriptResource : IRootResource
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
        public ScriptType Type { get; set; }

        [ProtoMember(5, AsReference = true)]
        public DataPointer Source;
    }
}