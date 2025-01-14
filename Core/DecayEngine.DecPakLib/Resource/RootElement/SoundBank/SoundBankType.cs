using ProtoBuf;

namespace DecayEngine.DecPakLib.Resource.RootElement.SoundBank
{
    public enum SoundBankType
    {
        [ProtoEnum]
        FmodMaster,
        [ProtoEnum]
        FmodStrings,
        [ProtoEnum]
        FmodSlave,
    }
}