using ProtoBuf;

namespace DecayEngine.DecPakLib.Resource.Expression.Property.Reference
{
    [ProtoContract]
    public class ResourceReferenceExpression : IPropertyExpression
    {
        [ProtoMember(1)]
        public string ReferenceId { get; set; }
    }
}