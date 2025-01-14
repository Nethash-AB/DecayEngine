using DecayEngine.DecPakLib.Resource.Expression.Query.Initiator;
using DecayEngine.DecPakLib.Resource.Structure.Component;
using ProtoBuf;

namespace DecayEngine.DecPakLib.Resource.Expression.Statement.Component.ComponentImplementations.Mesh
{
    [ProtoContract]
    public class CreateMeshComponentExpression : CreateComponentExpression
    {
        [ProtoIgnore]
        public override ComponentType ComponentType => ComponentType.Mesh;

        [ProtoMember(5)]
        public IQueryInitiatorExpression Material { get; set; }

        [ProtoMember(6)]
        public IQueryInitiatorExpression ShaderProgram { get; set; }

        [ProtoMember(7)]
        public IQueryInitiatorExpression Camera { get; set; }
    }
}