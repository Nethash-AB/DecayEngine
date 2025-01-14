using DecayEngine.DecPakLib.Resource.Expression.Property;
using DecayEngine.DecPakLib.Resource.Structure.Component;
using ProtoBuf;

namespace DecayEngine.DecPakLib.Resource.Expression.Statement.Component.ComponentImplementations.Light
{
    [ProtoContract]
    public class CreateLightSpotComponentExpression : CreateLightComponentExpression
    {
        [ProtoIgnore]
        public override ComponentType ComponentType => ComponentType.LightSpot;

        [ProtoIgnore]
        public override IPropertyExpression Template
        {
            get => null;
            set { }
        }

        [ProtoMember(9)]
        public float Radius { get; set; }

        [ProtoMember(10)]
        public float CutoffAngle { get; set; }
    }
}