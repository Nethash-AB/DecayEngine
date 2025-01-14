using System.Collections.Generic;
using DecayEngine.DecPakLib.Platform;
using DecayEngine.DecPakLib.Pointer;
using DecayEngine.DecPakLib.Resource.Expression.Property;
using ProtoBuf;

namespace DecayEngine.DecPakLib.Resource.RootElement.ShaderProgram
{
    [ProtoContract]
    public class ShaderProgramResource : IRootResource
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
        public IPropertyExpression VertexShader { get; set; }
        [ProtoMember(5)]
        public IPropertyExpression GeometryShader { get; set; }
        [ProtoMember(6)]
        public IPropertyExpression FragmentShader { get; set; }
    }
}