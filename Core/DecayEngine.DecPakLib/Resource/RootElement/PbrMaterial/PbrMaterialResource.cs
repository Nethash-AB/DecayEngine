using System.Collections.Generic;
using DecayEngine.DecPakLib.Platform;
using DecayEngine.DecPakLib.Pointer;
using DecayEngine.DecPakLib.Resource.Expression.Property;
using DecayEngine.DecPakLib.Resource.Structure.Math;
using ProtoBuf;

namespace DecayEngine.DecPakLib.Resource.RootElement.PbrMaterial
{
    [ProtoContract]
    public class PbrMaterialResource : IRootResource
    {
        [ProtoMember(1)]
        public string Id { get; set; }
        [ProtoMember(2)]
        public string MetaFilePath { get; set; }
        [ProtoMember(3)]
        public TargetPlatform TargetPlatforms { get; set; }

        [ProtoIgnore]
        public List<ByReference<DataPointer>> Pointers => new List<ByReference<DataPointer>>();

        [ProtoMember(4)]
        public IPropertyExpression AlbedoTexture { get; set; }
        [ProtoMember(5)]
        public Vector4Structure AlbedoColor { get; set; }

        [ProtoMember(6)]
        public IPropertyExpression NormalTexture { get; set; }

        [ProtoMember(7)]
        public IPropertyExpression MetallicityTexture { get; set; }
        [ProtoMember(8)]
        public float MetallicityFactor { get; set; }

        [ProtoMember(9)]
        public IPropertyExpression RoughnessTexture { get; set; }
        [ProtoMember(10)]
        public float RoughnessFactor { get; set; }

        [ProtoMember(11)]
        public IPropertyExpression EmissionTexture { get; set; }
        [ProtoMember(12)]
        public Vector4Structure EmissionColor { get; set; }
    }
}