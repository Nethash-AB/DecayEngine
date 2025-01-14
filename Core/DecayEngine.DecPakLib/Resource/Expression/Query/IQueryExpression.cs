using DecayEngine.DecPakLib.Resource.Expression.Query.Collection;
using DecayEngine.DecPakLib.Resource.Expression.Query.Initiator;
using DecayEngine.DecPakLib.Resource.Expression.Query.Single;
using ProtoBuf;

namespace DecayEngine.DecPakLib.Resource.Expression.Query
{
    [ProtoContract]
    [ProtoInclude(3000, typeof(IQueryCollectionExpression))]
    [ProtoInclude(3001, typeof(IQueryInitiatorExpression))]
    [ProtoInclude(3002, typeof(IQuerySingleExpression))]
    public interface IQueryExpression : IExpression
    {
        [ProtoIgnore]
        IQueryExpression Next { get; }
    }
}