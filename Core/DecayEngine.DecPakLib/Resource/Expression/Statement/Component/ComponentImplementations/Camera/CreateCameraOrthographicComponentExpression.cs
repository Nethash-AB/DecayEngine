using DecayEngine.DecPakLib.Resource.Expression.Property;
using DecayEngine.DecPakLib.Resource.Structure.Component;
using ProtoBuf;

namespace DecayEngine.DecPakLib.Resource.Expression.Statement.Component.ComponentImplementations.Camera
{
    [ProtoContract]
    public class CreateCameraOrthographicComponentExpression : CreateCameraComponentExpression
    {
        [ProtoIgnore]
        public override ComponentType ComponentType => ComponentType.CameraOrtho;

        [ProtoIgnore]
        public override IPropertyExpression Template
        {
            get => null;
            set { }
        }
    }
}