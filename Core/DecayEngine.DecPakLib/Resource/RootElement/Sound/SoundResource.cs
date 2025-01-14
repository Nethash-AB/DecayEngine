using System.Collections.Generic;
using DecayEngine.DecPakLib.Platform;
using DecayEngine.DecPakLib.Pointer;
using DecayEngine.DecPakLib.Resource.Expression.Property;
using ProtoBuf;

namespace DecayEngine.DecPakLib.Resource.RootElement.Sound
{
    [ProtoContract]
    public class SoundResource : IRootResource
    {
        [ProtoMember(1)]
        public string Id { get; set; }
        [ProtoMember(2)]
        public string MetaFilePath { get; set; }
        [ProtoMember(3)]
        public TargetPlatform TargetPlatforms { get; set; }

        [ProtoIgnore]
        public List<ByReference<DataPointer>> Pointers => new List<ByReference<DataPointer>>();

        [ProtoMember(4)]
        public IPropertyExpression Bank { get; set; }
        [ProtoMember(5)]
        public string Event { get; set; }
    }
}