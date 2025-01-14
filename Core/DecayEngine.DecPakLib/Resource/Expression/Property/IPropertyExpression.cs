using DecayEngine.DecPakLib.Resource.Expression.Property.Reference;
using ProtoBuf;

namespace DecayEngine.DecPakLib.Resource.Expression.Property
{
    [ProtoContract]
    [ProtoInclude(3000, typeof(ResourceReferenceExpression))]
    public interface IPropertyExpression : IExpression
    {
    }
}