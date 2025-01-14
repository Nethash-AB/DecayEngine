using DecayEngine.DecPakLib.Resource.Structure.Component;
using DecayEngine.DecPakLib.Resource.Structure.Transform;
using ProtoBuf;

namespace DecayEngine.DecPakLib.Resource.Expression.Statement.Component.ComponentImplementations.Sound
{
    [ProtoContract]
    public class CreateSoundComponentExpression : CreateComponentExpression
    {
        [ProtoIgnore]
        public override ComponentType ComponentType => ComponentType.Sound;

        [ProtoMember(4)]
        public TransformStructure Transform { get; set; }
        [ProtoMember(5)]
        public bool AutoPlayOnActive { get; set; }
    }
}