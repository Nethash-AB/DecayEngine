using ProtoBuf;

namespace DecayEngine.DecPakLib.Resource.RootElement.Shader
{
    public enum ShaderLanguage
    {
        [ProtoEnum]
        Glsl,
        [ProtoEnum]
        GlslEs,
        [ProtoEnum]
        Hlsl
    }
}