using ProtoBuf;

namespace DecayEngine.DecPakLib.Resource.Structure.Transform
{
    [ProtoContract]
    public enum TransformMode
    {
        [ProtoEnum]
        NotSet,
        [ProtoEnum]
        Absolute,
        [ProtoEnum]
        WorldSpace,
        [ProtoEnum]
        OrthoRelative
    }
}