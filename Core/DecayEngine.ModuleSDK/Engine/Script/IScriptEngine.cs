using System.Threading.Tasks;
using DecayEngine.ModuleSDK.Capability;

namespace DecayEngine.ModuleSDK.Engine.Script
{
    public interface IScriptEngine : IMultiThreadedEngine, IMarshaller
    {
        Task InjectExports(ScriptExports exports);
    }

    public interface IScriptEngine<in TOptions> : IScriptEngine, IMultiThreadedEngine<TOptions> where TOptions : IEngineOptions
    {
    }
}