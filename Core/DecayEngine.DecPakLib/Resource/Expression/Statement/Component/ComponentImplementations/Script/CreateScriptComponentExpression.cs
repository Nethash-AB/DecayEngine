using System.Collections.Generic;
using DecayEngine.DecPakLib.Resource.Structure.Component;
using DecayEngine.DecPakLib.Resource.Structure.Component.Script;
using ProtoBuf;

namespace DecayEngine.DecPakLib.Resource.Expression.Statement.Component.ComponentImplementations.Script
{
    [ProtoContract]
    public class CreateScriptComponentExpression : CreateComponentExpression
    {
        [ProtoIgnore]
        public override ComponentType ComponentType => ComponentType.Script;

        [ProtoMember(4)]
        public List<ScriptInjection> Injections { get; set; }
    }
}