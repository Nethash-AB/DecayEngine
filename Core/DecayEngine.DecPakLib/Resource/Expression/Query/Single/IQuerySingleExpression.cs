using ProtoBuf;

namespace DecayEngine.DecPakLib.Resource.Expression.Query.Single
{
    [ProtoContract]
    [ProtoInclude(4000, typeof(SelectParentExpression))]
    public interface IQuerySingleExpression : IQueryExpression
    {

    }
}