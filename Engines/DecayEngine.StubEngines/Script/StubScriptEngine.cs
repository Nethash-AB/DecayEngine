using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DecayEngine.ModuleSDK.Component;
using DecayEngine.ModuleSDK.Engine.Script;
using DecayEngine.ModuleSDK.Threading;

namespace DecayEngine.StubEngines.Script
{
    public class StubScriptEngine : IScriptEngine<StubEngineOptions>
    {
        public IEngineThread EngineThread { get; private set; }

        public List<IComponentFactory> ComponentFactories { get; }
        public ScriptExports ScriptExports { get; }

        public StubScriptEngine()
        {
            ComponentFactories = new List<IComponentFactory>();
            ScriptExports = new ScriptExports();
        }

        public Task Init(StubEngineOptions options)
        {
            EngineThread = new ManagedEngineThread("", 128);
            EngineThread.Run();

            return Task.CompletedTask;
        }

        public Task Shutdown()
        {
            EngineThread.Stop();
            return Task.CompletedTask;
        }

        public Task InjectExports(ScriptExports exports)
        {
            return Task.CompletedTask;
        }

        public object MarshalTo(object obj)
        {
            return obj;
        }

        public T MarshalFrom<T>(object obj) where T : class, new()
        {
            return (T) obj;
        }
    }
}