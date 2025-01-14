using System.Collections.Generic;
using DecayEngine.DecPakLib.Platform;
using DecayEngine.DecPakLib.Pointer;
using DecayEngine.DecPakLib.Resource.Expression.Property;
using ProtoBuf;

namespace DecayEngine.DecPakLib.Resource.RootElement.SoundBank
{
    [ProtoContract]
    public class SoundBankResource : IRootResource
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
        public SoundBankType Type { get; set; }

        [ProtoMember(5, AsReference = true)]
        public DataPointer Source;

        [ProtoMember(6)]
        public List<IPropertyExpression> Requires { get; set; }
    }
}