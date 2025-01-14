using DecayEngine.DecPakLib.Resource.Structure.Component;
using ProtoBuf;

namespace DecayEngine.DecPakLib.Resource.Expression.Statement.Component.ComponentImplementations.ShaderProgram
{
    [ProtoContract]
    public class CreateShaderProgramComponentExpression : CreateComponentExpression
    {
        [ProtoIgnore]
        public override ComponentType ComponentType => ComponentType.ShaderProgram;
    }
}