using DecayEngine.DecPakLib.Resource.Expression.Property;
using DecayEngine.DecPakLib.Resource.Structure.Common;
using DecayEngine.DecPakLib.Resource.Structure.Common.PropertySheet;
using ProtoBuf;

namespace DecayEngine.DecPakLib.Resource.RootElement.PostProcessing
{
    [ProtoContract]
    public class PostProcessingStage : IResource
    {
        [ProtoMember(2000)]
        public string Name { get; set; }

        [ProtoMember(2001)]
        public IPropertyExpression ShaderProgram { get; set; }

        [ProtoMember(2002)]
        public float[] Kernel { get; set; }

        [ProtoMember(2003)]
        public PropertySheetStructure Properties { get; set; }
    }
}