using DecayEngine.DecPakLib.Resource.Expression.Property;
using DecayEngine.DecPakLib.Resource.Expression.Query.Initiator;
using DecayEngine.DecPakLib.Resource.Structure.Component;
using DecayEngine.DecPakLib.Resource.Structure.Math;
using ProtoBuf;

namespace DecayEngine.DecPakLib.Resource.Expression.Statement.Component.ComponentImplementations.Light
{
    [ProtoContract]
    [ProtoInclude(5000, typeof(CreateLightPointComponentExpression))]
    [ProtoInclude(5001, typeof(CreateLightDirectionalComponentExpression))]
    [ProtoInclude(5002, typeof(CreateLightSpotComponentExpression))]
    public abstract class CreateLightComponentExpression : CreateComponentExpression
    {
        [ProtoIgnore]
        public override ComponentType ComponentType => ComponentType.Light;

        [ProtoIgnore]
        public override IPropertyExpression Template
        {
            get => null;
            set { }
        }

        [ProtoMember(7)]
        public IQueryInitiatorExpression Camera { get; set; }
        [ProtoMember(8)]
        public ColorStructure Color { get; set; }
        [ProtoMember(9)]
        public float Strength { get; set; }
    }
}