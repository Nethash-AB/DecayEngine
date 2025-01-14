using DecayEngine.DecPakLib.Resource.Expression.Property;
using DecayEngine.DecPakLib.Resource.Structure.Component;
using DecayEngine.DecPakLib.Resource.Structure.Transform;
using ProtoBuf;

namespace DecayEngine.DecPakLib.Resource.Expression.Statement.Component.ComponentImplementations.Camera
{
    [ProtoContract]
    [ProtoInclude(5000, typeof(CreateCameraPerspectiveComponentExpression))]
    [ProtoInclude(5001, typeof(CreateCameraOrthographicComponentExpression))]
    public abstract class CreateCameraComponentExpression : CreateComponentExpression
    {
        [ProtoIgnore]
        public override ComponentType ComponentType => ComponentType.Camera;

        [ProtoIgnore]
        public override IPropertyExpression Template
        {
            get => null;
            set { }
        }

        [ProtoMember(4)]
        public TransformStructure Transform { get; set; }
        [ProtoMember(7)]
        public IPropertyExpression PostProcessingPreset { get; set; }
        [ProtoMember(8)]
        public float ZNear { get; set; }
        [ProtoMember(9)]
        public float ZFar { get; set; }
        [ProtoMember(10)]
        public bool IsAudioListener { get; set; }
        [ProtoMember(11)]
        public bool IsPbr { get; set; }
        [ProtoMember(12)]
        public IPropertyExpression EnvironmentTexture { get; set; }
        [ProtoMember(13)]
        public bool RenderToScreen { get; set; }
    }
}