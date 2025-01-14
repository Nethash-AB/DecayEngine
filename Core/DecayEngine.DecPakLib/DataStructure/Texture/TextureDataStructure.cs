using System.Collections.Generic;
using System.IO;
using ProtoBuf;
using ProtoBuf.Meta;

namespace DecayEngine.DecPakLib.DataStructure.Texture
{
    [ProtoContract]
    public class TextureDataStructure
    {
        [ProtoMember(1)]
        public TextureDataFormat CompressedFormat { get; set; }
        [ProtoMember(2)]
        public TextureDataFormat UncompressedFormat { get; set; }
        [ProtoMember(3)]
        public List<TextureMipMapDataStructure> CompressedMipMaps { get; set; }
        [ProtoMember(4)]
        public List<TextureMipMapDataStructure> UncompressedMipMaps { get; set; }

        public MemoryStream Serialize()
        {
            MemoryStream ms = new MemoryStream();

            RuntimeTypeModel runtimeTypeModel = TypeModel.Create();
            runtimeTypeModel.AutoCompile = false;
            runtimeTypeModel.AutoAddMissingTypes = true;
            runtimeTypeModel.UseImplicitZeroDefaults = true;
            runtimeTypeModel.AutoAddProtoContractTypesOnly = true;

            runtimeTypeModel.Serialize(ms, this);
            return ms;
        }

        public void Deserialize(Stream stream)
        {
            RuntimeTypeModel runtimeTypeModel = TypeModel.Create();
            runtimeTypeModel.AutoCompile = false;
            runtimeTypeModel.AutoAddMissingTypes = true;
            runtimeTypeModel.UseImplicitZeroDefaults = true;
            runtimeTypeModel.AutoAddProtoContractTypesOnly = true;

            runtimeTypeModel.Deserialize(stream, this, typeof(TextureDataStructure));
        }
    }
}