using System.Collections.Generic;
using DecayEngine.DecPakLib.Platform;
using DecayEngine.DecPakLib.Pointer;
using DecayEngine.DecPakLib.Resource.Structure.Component.Font;
using DecayEngine.DecPakLib.Resource.Structure.Math;
using ProtoBuf;

namespace DecayEngine.DecPakLib.Resource.RootElement.Font
{
    [ProtoContract]
    public class FontResource : IRootResource
    {
        [ProtoMember(1)]
        public string Id { get; set; }
        [ProtoMember(2)]
        public string MetaFilePath { get; set; }
        [ProtoMember(3)]
        public TargetPlatform TargetPlatforms { get; set; }

        [ProtoIgnore]
        public List<ByReference<DataPointer>> Pointers => new List<ByReference<DataPointer>>{() => ref Source};

        [ProtoMember(4)]
        public List<string> ExtraCharacters { get; set; }
        [ProtoMember(5)]
        public bool OutlineOnly { get; set; }
        [ProtoMember(6)]
        public float OutlineThickness { get; set; }

        [ProtoMember(7, AsReference = true)]
        public DataPointer Source;

        [ProtoMember(8)]
        public List<Glyph> Glyphs { get; set; }

        [ProtoMember(9)]
        public List<FontMipMap> MipMaps { get; set; }
    }
}