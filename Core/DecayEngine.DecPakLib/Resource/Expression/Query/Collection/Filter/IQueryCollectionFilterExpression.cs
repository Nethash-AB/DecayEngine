using ProtoBuf;

namespace DecayEngine.DecPakLib.Resource.Expression.Query.Collection.Filter
{
    [ProtoContract]
    [ProtoInclude(5000, typeof(FilterByComponentTypeExpression))]
    [ProtoInclude(5001, typeof(FilterByNameExpression))]
    public interface IQueryCollectionFilterExpression : IQueryCollectionExpression
    {
        
    }
}