using DecayEngine.DecPakLib.Resource.Expression.Property;
using DecayEngine.DecPakLib.Resource.Structure.Component;
using ProtoBuf;

namespace DecayEngine.DecPakLib.Resource.Expression.Statement.Component.ComponentImplementations.Light
{
    [ProtoContract]
    public class CreateLightDirectionalComponentExpression : CreateLightComponentExpression
    {
        [ProtoIgnore]
        public override ComponentType ComponentType => ComponentType.LightDirectional;

        [ProtoIgnore]
        public override IPropertyExpression Template
        {
            get => null;
            set { }
        }
    }
}