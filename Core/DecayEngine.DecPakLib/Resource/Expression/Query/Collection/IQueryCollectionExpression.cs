using DecayEngine.DecPakLib.Resource.Expression.Query.Collection.Filter;
using DecayEngine.DecPakLib.Resource.Expression.Query.Collection.Terminator;
using ProtoBuf;

namespace DecayEngine.DecPakLib.Resource.Expression.Query.Collection
{
    [ProtoContract]
    [ProtoInclude(4000, typeof(SelectChildrenExpression))]
    [ProtoInclude(4001, typeof(SelectComponentsExpression))]
    [ProtoInclude(4002, typeof(IQueryCollectionFilterExpression))]
    [ProtoInclude(4003, typeof(IQueryCollectionTerminatorExpression))]
    public interface IQueryCollectionExpression : IQueryExpression
    {
        
    }
}