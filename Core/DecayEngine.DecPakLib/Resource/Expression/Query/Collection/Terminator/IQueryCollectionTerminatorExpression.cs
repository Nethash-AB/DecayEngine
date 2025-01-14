using ProtoBuf;

namespace DecayEngine.DecPakLib.Resource.Expression.Query.Collection.Terminator
{
    [ProtoContract]
    [ProtoInclude(5000, typeof(SelectFirstCollectionTerminatorExpression))]
    [ProtoInclude(5001, typeof(SelectAllCollectionTerminatorExpression))]
    [ProtoInclude(5002, typeof(SelectFrameBufferTerminatorExpression))]
    public interface IQueryCollectionTerminatorExpression : IQueryCollectionExpression
    {
    }
}