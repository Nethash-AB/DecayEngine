using ProtoBuf;

namespace DecayEngine.DecPakLib.Resource.Structure.Component
{
    [ProtoContract]
    public enum ComponentType
    {
        [ProtoEnum]
        AnimatedMaterial,
        [ProtoEnum]
        PbrMaterial,
        [ProtoEnum]
        Shader,
        [ProtoEnum]
        ShaderProgram,
        [ProtoEnum]
        Script,
        [ProtoEnum]
        AnimatedSprite,
        [ProtoEnum]
        RenderTargetSprite,
        [ProtoEnum]
        Mesh,
        [ProtoEnum]
        TextSprite,
        [ProtoEnum]
        Camera,
        [ProtoEnum]
        CameraPersp,
        [ProtoEnum]
        CameraOrtho,
        [ProtoEnum]
        SoundBank,
        [ProtoEnum]
        Sound,
        [ProtoEnum]
        RigidBody,
        [ProtoEnum]
        Light,
        [ProtoEnum]
        LightPoint,
        [ProtoEnum]
        LightDirectional,
        [ProtoEnum]
        LightSpot
    }
}