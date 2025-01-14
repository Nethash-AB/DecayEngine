using System;
using DecayEngine.DecPakLib.Bundle;
using DecayEngine.ModuleSDK.Engine;
using DecayEngine.ModuleSDK.Engine.Extension;
using DecayEngine.ModuleSDK.Engine.Input;
using DecayEngine.ModuleSDK.Engine.Physics;
using DecayEngine.ModuleSDK.Engine.Render;
using DecayEngine.ModuleSDK.Engine.Script;
using DecayEngine.ModuleSDK.Engine.Sound;
using DecayEngine.ModuleSDK.Logging;

namespace DecayEngine.ModuleSDK.Builder
{
    public interface IGameBuilder
    {
        IGameBuilder AddResourceBundle(ResourceBundle bundle);

        IGameBuilder UseScriptEngine<T, TOptions>(Action<TOptions> options = null)
            where T : class, IScriptEngine<TOptions>, new()
            where TOptions : class, IEngineOptions, new();

        IGameBuilder UseRenderEngine<T, TOptions>(Action<TOptions> options = null)
            where T : class, IRenderEngine<TOptions>, new()
            where TOptions : class, IEngineOptions, new();

        IGameBuilder UseSoundEngine<T, TOptions>(Action<TOptions> options = null)
            where T : class, ISoundEngine<TOptions>, new()
            where TOptions : class, IEngineOptions, new();

        IGameBuilder UsePhysicsEngine<T, TOptions>(Action<TOptions> options = null)
            where T : class, IPhysicsEngine<TOptions>, new()
            where TOptions : class, IEngineOptions, new();

        IGameBuilder AddInputProvider<T, TOptions>(Action<TOptions> options = null)
            where T : class, IInputProvider<TOptions>, new()
            where TOptions : class, IEngineOptions, new();

        IGameBuilder AddExtension<T, TOptions>(Action<TOptions> options = null)
            where T : class, IEngineExtension<TOptions>, new()
            where TOptions : class, IEngineOptions, new();

        IGameBuilder AddLogger<T, TOptions>(Action<TOptions> options = null)
            where T : class, ILogger<TOptions>, new()
            where TOptions : class, ILoggerOptions, new();

        IGameBuilder WithInitScene(string sceneId);

        IGameBuilderOutput Build();
    }
}