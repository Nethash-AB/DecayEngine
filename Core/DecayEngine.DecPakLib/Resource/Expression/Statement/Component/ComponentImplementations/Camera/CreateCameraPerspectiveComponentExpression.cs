using DecayEngine.DecPakLib.Resource.Expression.Property;
using DecayEngine.DecPakLib.Resource.Structure.Component;
using ProtoBuf;

namespace DecayEngine.DecPakLib.Resource.Expression.Statement.Component.ComponentImplementations.Camera
{
    [ProtoContract]
    public class CreateCameraPerspectiveComponentExpression : CreateCameraComponentExpression
    {
        [ProtoIgnore]
        public override ComponentType ComponentType => ComponentType.CameraPersp;

        [ProtoIgnore]
        public override IPropertyExpression Template
        {
            get => null;
            set { }
        }

        [ProtoMember(12)]
        public float FieldOfView { get; set; }
    }
}