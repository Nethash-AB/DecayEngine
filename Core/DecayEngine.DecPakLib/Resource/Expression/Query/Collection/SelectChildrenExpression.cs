using ProtoBuf;

namespace DecayEngine.DecPakLib.Resource.Expression.Query.Collection
{
    [ProtoContract]
    public class SelectChildrenExpression : IQueryCollectionExpression
    {
        [ProtoMember(1)]
        public IQueryExpression Next { get; set; }
    }
}