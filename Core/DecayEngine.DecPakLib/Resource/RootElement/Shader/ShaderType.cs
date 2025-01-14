using ProtoBuf;

namespace DecayEngine.DecPakLib.Resource.RootElement.Shader
{
    public enum ShaderType
    {
        [ProtoEnum]
        Vertex,
        [ProtoEnum]
        Geometry,
        [ProtoEnum]
        Fragment
    }
}