using DecayEngine.ModuleSDK.Threading;

namespace DecayEngine.ModuleSDK.Engine
{
    public interface IMultiThreadedEngine : IEngine
    {
        IEngineThread EngineThread { get; }
    }

    public interface IMultiThreadedEngine<in TOptions> : IMultiThreadedEngine, IEngine<TOptions> where TOptions : IEngineOptions
    {

    }
}