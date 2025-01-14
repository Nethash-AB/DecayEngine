using DecayEngine.DecPakLib.Resource.Expression.Statement.Component;
using DecayEngine.DecPakLib.Resource.Expression.Statement.GameObject;
using ProtoBuf;

namespace DecayEngine.DecPakLib.Resource.Expression.Statement
{
    [ProtoContract]
    [ProtoInclude(3000, typeof(CreateComponentExpression))]
    [ProtoInclude(3001, typeof(CreateGameObjectExpression))]
    public interface IStatementExpression : IExpression
    {
        
    }
}