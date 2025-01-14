using System.Collections.Generic;
using DecayEngine.DecPakLib.Platform;
using DecayEngine.DecPakLib.Pointer;
using ProtoBuf;

namespace DecayEngine.DecPakLib.Resource.RootElement.Texture3D
{
    [ProtoContract]
    public class Texture3DResource : IRootResource
    {
        [ProtoMember(1)]
        public string Id { get; set; }
        [ProtoMember(2)]
        public string MetaFilePath { get; set; }
        [ProtoMember(3)]
        public TargetPlatform TargetPlatforms { get; set; }

        [ProtoIgnore]
        public List<ByReference<DataPointer>> Pointers => new List<ByReference<DataPointer>>
        {
            () => ref SourceBack,
            () => ref SourceDown,
            () => ref SourceFront,
            () => ref SourceLeft,
            () => ref SourceRight,
            () => ref SourceUp
        };

        [ProtoMember(5, AsReference = true)]
        public DataPointer SourceBack;
        [ProtoMember(6, AsReference = true)]
        public DataPointer SourceDown;
        [ProtoMember(7, AsReference = true)]
        public DataPointer SourceFront;
        [ProtoMember(8, AsReference = true)]
        public DataPointer SourceLeft;
        [ProtoMember(9, AsReference = true)]
        public DataPointer SourceRight;
        [ProtoMember(10, AsReference = true)]
        public DataPointer SourceUp;
    }
}