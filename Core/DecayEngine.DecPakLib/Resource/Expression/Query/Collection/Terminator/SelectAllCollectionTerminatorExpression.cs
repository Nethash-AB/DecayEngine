using ProtoBuf;

namespace DecayEngine.DecPakLib.Resource.Expression.Query.Collection.Terminator
{
    [ProtoContract]
    public class SelectAllCollectionTerminatorExpression : IQueryCollectionTerminatorExpression
    {
        [ProtoMember(1)]
        public IQueryExpression Next => null;
    }
}