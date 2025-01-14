using ProtoBuf;

namespace DecayEngine.DecPakLib.Resource.Expression.Query.Collection.Terminator
{
    [ProtoContract]
    public class SelectFirstCollectionTerminatorExpression : IQueryCollectionTerminatorExpression
    {
        [ProtoMember(1)]
        public IQueryExpression Next => null;
    }
}