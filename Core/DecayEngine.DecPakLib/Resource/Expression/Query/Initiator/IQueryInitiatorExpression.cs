using ProtoBuf;

namespace DecayEngine.DecPakLib.Resource.Expression.Query.Initiator
{
    [ProtoContract]
    [ProtoInclude(4000, typeof(SelectActiveSceneExpression))]
    [ProtoInclude(4001, typeof(SelectThisExpression))]
    [ProtoInclude(4002, typeof(SelectGlobalExpression))]
    [ProtoInclude(4003, typeof(SelectRootExpression))]
    public interface IQueryInitiatorExpression : IQueryExpression
    {

    }
}