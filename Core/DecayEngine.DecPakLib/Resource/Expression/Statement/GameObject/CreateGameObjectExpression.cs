using System.Collections.Generic;
using DecayEngine.DecPakLib.Resource.Expression.Property;
using DecayEngine.DecPakLib.Resource.Structure.Transform;
using ProtoBuf;

namespace DecayEngine.DecPakLib.Resource.Expression.Statement.GameObject
{
    [ProtoContract]
    public class CreateGameObjectExpression : IStatementExpression
    {
        [ProtoMember(1)]
        public string Name { get; set; }
        [ProtoMember(2)]
        public bool Active { get; set; }
        [ProtoMember(3)]
        public IPropertyExpression Template { get; set; }
        [ProtoMember(4)]
        public List<IStatementExpression> Children { get; set; }
        [ProtoMember(5)]
        public TransformStructure Transform { get; set; }
    }
}