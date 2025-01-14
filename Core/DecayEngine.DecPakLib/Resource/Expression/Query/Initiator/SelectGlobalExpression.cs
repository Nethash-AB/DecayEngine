using ProtoBuf;

namespace DecayEngine.DecPakLib.Resource.Expression.Query.Initiator
{
    [ProtoContract]
    public class SelectGlobalExpression : IQueryInitiatorExpression
    {
        [ProtoMember(1)]
        public IQueryExpression Next { get; set; }
    }
}