using ProtoBuf;

namespace DecayEngine.DecPakLib.Resource.Expression.Query.Single
{
    [ProtoContract]
    public class SelectParentExpression : IQuerySingleExpression
    {
        [ProtoMember(1)]
        public IQueryExpression Next { get; set; }
    }
}