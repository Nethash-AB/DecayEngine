using DecayEngine.DecPakLib.Resource.Structure.Component;
using ProtoBuf;

namespace DecayEngine.DecPakLib.Resource.Expression.Query.Collection.Filter
{
    [ProtoContract]
    public class FilterByComponentTypeExpression : IQueryCollectionFilterExpression
    {
        [ProtoMember(1)]
        public IQueryExpression Next { get; set; }

        [ProtoMember(2)]
        public ComponentType Type { get; set; }
    }
}