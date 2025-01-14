using DecayEngine.DecPakLib.Resource.Expression.Query.Initiator;
using DecayEngine.DecPakLib.Resource.Structure.Component;
using DecayEngine.DecPakLib.Resource.Structure.Component.TextSprite;
using DecayEngine.DecPakLib.Resource.Structure.Math;
using DecayEngine.DecPakLib.Resource.Structure.Transform;
using ProtoBuf;

namespace DecayEngine.DecPakLib.Resource.Expression.Statement.Component.ComponentImplementations.TextSprite
{
    [ProtoContract]
    public class CreateTextSpriteComponentExpression : CreateComponentExpression
    {
        [ProtoIgnore]
        public override ComponentType ComponentType => ComponentType.TextSprite;

        [ProtoMember(4)]
        public string Text { get; set; }
        [ProtoMember(5)]
        public TextSpriteAlignment Alignment { get; set; }
        [ProtoMember(6)]
        public ColorStructure Color { get; set; }
        [ProtoMember(7)]
        public float FontSize { get; set; }
        [ProtoMember(8)]
        public float CharacterSeparation { get; set; }
        [ProtoMember(9)]
        public float WhiteSpaceSeparation { get; set; }

        [ProtoMember(10)]
        public TransformStructure Transform { get; set; }
        [ProtoMember(11)]
        public IQueryInitiatorExpression ShaderProgram { get; set; }
        [ProtoMember(12)]
        public IQueryInitiatorExpression Camera { get; set; }
    }
}