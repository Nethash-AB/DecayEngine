using DecayEngine.DecPakLib.Resource.Expression.Property;
using DecayEngine.DecPakLib.Resource.Expression.Query.Initiator;
using DecayEngine.DecPakLib.Resource.Structure.Component;
using DecayEngine.DecPakLib.Resource.Structure.Transform;
using ProtoBuf;

namespace DecayEngine.DecPakLib.Resource.Expression.Statement.Component.ComponentImplementations.RenderTargetSprite
{
    [ProtoContract]
    public class CreateRenderTargetSpriteComponentExpression : CreateComponentExpression
    {
        [ProtoIgnore]
        public override ComponentType ComponentType => ComponentType.RenderTargetSprite;

        [ProtoIgnore]
        public override IPropertyExpression Template
        {
            get => null;
            set { }
        }

        [ProtoMember(4)]
        public TransformStructure Transform { get; set; }
        [ProtoMember(5)]
        public IQueryInitiatorExpression FrameBuffer { get; set; }
        [ProtoMember(6)]
        public IQueryInitiatorExpression ShaderProgram { get; set; }
        [ProtoMember(7)]
        public IQueryInitiatorExpression Camera { get; set; }
        [ProtoMember(8)]
        public bool MaintainAspectRatio { get; set; }
    }
}