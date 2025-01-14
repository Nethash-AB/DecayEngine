using ProtoBuf;

namespace DecayEngine.DecPakLib.Resource.Expression.Query.Collection.Filter
{
    [ProtoContract]
    public class FilterByNameExpression : IQueryCollectionFilterExpression
    {
        [ProtoMember(1)]
        public IQueryExpression Next { get; set; }

        [ProtoMember(2)]
        public string Name { get; set; }
    }
}