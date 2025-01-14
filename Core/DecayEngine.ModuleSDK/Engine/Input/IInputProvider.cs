namespace DecayEngine.ModuleSDK.Engine.Input
{
    public interface IInputProvider : IEngine
    {
    }

    public interface IInputProvider<in TOptions> : IEngine<TOptions>, IInputProvider
        where TOptions : IEngineOptions
    {
    }
}