using System.Collections.Generic;
using System.Threading.Tasks;
using DecayEngine.ModuleSDK.Component;
using DecayEngine.ModuleSDK.Engine.Script;

namespace DecayEngine.ModuleSDK.Engine
{
    public interface IEngine
    {
        List<IComponentFactory> ComponentFactories { get; }
        ScriptExports ScriptExports { get; }
        Task Shutdown();
    }

    public interface IEngine<in TOptions> : IEngine where TOptions : IEngineOptions
    {
        Task Init(TOptions options);
    }
}