using System;
using ProtoBuf;

namespace DecayEngine.DecPakLib.Resource.Structure.Component.TextSprite
{
    [Flags]
    [ProtoContract]
    public enum TextSpriteAlignment
    {
        [ProtoEnum]
        NotSet = 0,
        [ProtoEnum]
        HorizontalLeft = 1,
        [ProtoEnum]
        HorizontalCenter = 2,
        [ProtoEnum]
        HorizontalRight = 4,
        [ProtoEnum]
        VerticalTop = 8,
        [ProtoEnum]
        VerticalCenter = 16,
        [ProtoEnum]
        VerticalBottom = 32,
    }
}