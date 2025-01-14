using ProtoBuf;

namespace DecayEngine.DecPakLib.Resource.Expression.Query.Collection.Terminator
{
    [ProtoContract]
    public class SelectFrameBufferTerminatorExpression : IQueryCollectionTerminatorExpression
    {
        [ProtoMember(1)]
        public IQueryExpression Next => null;

        [ProtoMember(2)]
        public string Name { get; set; }
    }
}