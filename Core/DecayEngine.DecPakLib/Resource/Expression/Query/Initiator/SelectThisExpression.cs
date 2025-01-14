using ProtoBuf;

namespace DecayEngine.DecPakLib.Resource.Expression.Query.Initiator
{
    [ProtoContract]
    public class SelectThisExpression : IQueryInitiatorExpression
    {
        [ProtoMember(1)]
        public IQueryExpression Next { get; set; }
    }
}