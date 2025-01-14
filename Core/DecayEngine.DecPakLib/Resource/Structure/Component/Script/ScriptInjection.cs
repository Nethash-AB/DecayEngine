using DecayEngine.DecPakLib.Resource.Expression.Query.Initiator;
using ProtoBuf;

namespace DecayEngine.DecPakLib.Resource.Structure.Component.Script
{
    [ProtoContract]
    public class ScriptInjection : IResource
    {
        [ProtoMember(1)]
        public string Id { get; set; }

        [ProtoMember(2)]
        public IQueryInitiatorExpression Expression { get; set; }
    }
}