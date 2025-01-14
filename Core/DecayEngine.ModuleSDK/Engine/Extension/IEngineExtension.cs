namespace DecayEngine.ModuleSDK.Engine.Extension
{
    public interface IEngineExtension : IEngine
    {
    }

    public interface IEngineExtension<in TOptions> : IEngineExtension, IEngine<TOptions> where TOptions : IEngineOptions
    {
    }
}