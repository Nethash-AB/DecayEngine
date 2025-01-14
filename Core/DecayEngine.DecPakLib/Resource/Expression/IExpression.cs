using DecayEngine.DecPakLib.Resource.Expression.Property;
using DecayEngine.DecPakLib.Resource.Expression.Query;
using DecayEngine.DecPakLib.Resource.Expression.Statement;
using ProtoBuf;

namespace DecayEngine.DecPakLib.Resource.Expression
{
    [ProtoContract]
    [ProtoInclude(2000, typeof(IPropertyExpression))]
    [ProtoInclude(2001, typeof(IQueryExpression))]
    [ProtoInclude(2002, typeof(IStatementExpression))]
    public interface IExpression : IResource
    {
    }
}