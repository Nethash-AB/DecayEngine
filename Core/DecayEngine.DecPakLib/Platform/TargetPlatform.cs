using System;
using ProtoBuf;

namespace DecayEngine.DecPakLib.Platform
{
    [Flags]
    [ProtoContract(EnumPassthru = true)]
    public enum TargetPlatform : uint
    {
        [ProtoEnum]
        NotSet = 0,
        [ProtoEnum]
        WindowsNative = 1,
        [ProtoEnum]
        WindowsUwp = 2,
        [ProtoEnum]
        Linux = 4,
        [ProtoEnum]
        MacOs = 8,
        [ProtoEnum]
        Android = 16,
        [ProtoEnum]
        AppleIOs = 32,
        [ProtoEnum]
        Switch = 64,
        [ProtoEnum]
        Xbox = 128,
        [ProtoEnum]
        Playstation4 = 128,
        [ProtoEnum]
        Windows = WindowsNative | WindowsUwp,
        [ProtoEnum]
        Desktop = Windows | Linux | MacOs,
        [ProtoEnum]
        Mobile = Android | AppleIOs,
        [ProtoEnum]
        Consoles = Switch | Xbox | Playstation4,
        [ProtoEnum]
        All = ~0u
    }
}