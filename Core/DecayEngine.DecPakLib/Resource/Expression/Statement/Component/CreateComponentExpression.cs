using DecayEngine.DecPakLib.Resource.Expression.Property;
using DecayEngine.DecPakLib.Resource.Expression.Statement.Component.ComponentImplementations.AnimatedMaterial;
using DecayEngine.DecPakLib.Resource.Expression.Statement.Component.ComponentImplementations.AnimatedSprite;
using DecayEngine.DecPakLib.Resource.Expression.Statement.Component.ComponentImplementations.Camera;
using DecayEngine.DecPakLib.Resource.Expression.Statement.Component.ComponentImplementations.Light;
using DecayEngine.DecPakLib.Resource.Expression.Statement.Component.ComponentImplementations.Mesh;
using DecayEngine.DecPakLib.Resource.Expression.Statement.Component.ComponentImplementations.PbrMaterial;
using DecayEngine.DecPakLib.Resource.Expression.Statement.Component.ComponentImplementations.RenderTargetSprite;
using DecayEngine.DecPakLib.Resource.Expression.Statement.Component.ComponentImplementations.RigidBody;
using DecayEngine.DecPakLib.Resource.Expression.Statement.Component.ComponentImplementations.Script;
using DecayEngine.DecPakLib.Resource.Expression.Statement.Component.ComponentImplementations.ShaderProgram;
using DecayEngine.DecPakLib.Resource.Expression.Statement.Component.ComponentImplementations.Sound;
using DecayEngine.DecPakLib.Resource.Expression.Statement.Component.ComponentImplementations.SoundBank;
using DecayEngine.DecPakLib.Resource.Expression.Statement.Component.ComponentImplementations.TextSprite;
using DecayEngine.DecPakLib.Resource.Structure.Component;
using ProtoBuf;

namespace DecayEngine.DecPakLib.Resource.Expression.Statement.Component
{
    [ProtoContract]
    [ProtoInclude(4000, typeof(CreateCameraComponentExpression))]
    [ProtoInclude(4001, typeof(CreateAnimatedMaterialComponentExpression))]
    [ProtoInclude(4002, typeof(CreateRenderTargetSpriteComponentExpression))]
    [ProtoInclude(4003, typeof(CreateScriptComponentExpression))]
    [ProtoInclude(4004, typeof(CreateShaderProgramComponentExpression))]
    [ProtoInclude(4005, typeof(CreateAnimatedSpriteComponentExpression))]
    [ProtoInclude(4006, typeof(CreateTextSpriteComponentExpression))]
    [ProtoInclude(4007, typeof(CreateSoundComponentExpression))]
    [ProtoInclude(4008, typeof(CreateSoundBankComponentExpression))]
    [ProtoInclude(4009, typeof(CreateRigidBodyComponentExpression))]
    [ProtoInclude(4010, typeof(CreateMeshComponentExpression))]
    [ProtoInclude(4011, typeof(CreatePbrMaterialComponentExpression))]
    [ProtoInclude(4012, typeof(CreateLightComponentExpression))]
    public abstract class CreateComponentExpression : IStatementExpression
    {
        [ProtoMember(1)]
        public string Name { get; set; }
        [ProtoMember(2)]
        public bool Active { get; set; }
        [ProtoIgnore]
        public abstract ComponentType ComponentType { get; }
        [ProtoMember(3)]
        public virtual IPropertyExpression Template { get; set; }
    }
}