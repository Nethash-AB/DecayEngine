using ProtoBuf;

namespace DecayEngine.DecPakLib.Resource.Expression.Query.Initiator
{
    [ProtoContract]
    public class SelectRootExpression : IQueryInitiatorExpression
    {
        [ProtoMember(1)]
        public IQueryExpression Next { get; set; }
    }
}