using DecayEngine.DecPakLib.Resource.Expression.Property;
using DecayEngine.DecPakLib.Resource.Expression.Query.Initiator;
using DecayEngine.DecPakLib.Resource.Structure.Component;
using DecayEngine.DecPakLib.Resource.Structure.Transform;
using ProtoBuf;

namespace DecayEngine.DecPakLib.Resource.Expression.Statement.Component.ComponentImplementations.AnimatedSprite
{
    [ProtoContract]
    public class CreateAnimatedSpriteComponentExpression : CreateComponentExpression
    {
        [ProtoIgnore]
        public override ComponentType ComponentType => ComponentType.AnimatedSprite;

        [ProtoIgnore]
        public override IPropertyExpression Template
        {
            get => null;
            set { }
        }

        [ProtoMember(4)]
        public TransformStructure Transform { get; set; }
        [ProtoMember(5)]
        public IQueryInitiatorExpression Material { get; set; }
        [ProtoMember(6)]
        public IQueryInitiatorExpression ShaderProgram { get; set; }
        [ProtoMember(7)]
        public IQueryInitiatorExpression Camera { get; set; }
    }
}