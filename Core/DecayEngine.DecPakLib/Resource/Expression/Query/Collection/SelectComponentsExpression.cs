using ProtoBuf;

namespace DecayEngine.DecPakLib.Resource.Expression.Query.Collection
{
    [ProtoContract]
    public class SelectComponentsExpression : IQueryCollectionExpression
    {
        [ProtoMember(1)]
        public IQueryExpression Next { get; set; }
    }
}