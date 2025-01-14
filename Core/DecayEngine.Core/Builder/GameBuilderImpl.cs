using System;
using System.Collections.Generic;
using DecayEngine.DecPakLib.Bundle;
using DecayEngine.ModuleSDK.Builder;
using DecayEngine.ModuleSDK.Engine;
using DecayEngine.ModuleSDK.Engine.Extension;
using DecayEngine.ModuleSDK.Engine.Input;
using DecayEngine.ModuleSDK.Engine.Physics;
using DecayEngine.ModuleSDK.Engine.Render;
using DecayEngine.ModuleSDK.Engine.Script;
using DecayEngine.ModuleSDK.Engine.Sound;
using DecayEngine.ModuleSDK.Logging;

namespace DecayEngine.Core.Builder
{
    public class GameBuilderImpl : IGameBuilder
    {
        private readonly List<ResourceBundle> _resourceBundles;

        private Type _scriptEngineType;
        private IEngineOptions _scriptEngineOptions;

        private Type _renderEngineType;
        private IEngineOptions _renderEngineOptions;

        private Type _soundEngineType;
        private IEngineOptions _soundEngineOptions;

        private Type _physicsEngineType;
        private IEngineOptions _physicsEngineOptions;

        private readonly List<Tuple<Type, IEngineOptions>> _inputProviders;
        private readonly List<Tuple<Type, IEngineOptions>> _extensions;
        private readonly List<Tuple<Type, ILoggerOptions>> _loggers;
        private string _initScene;

        public GameBuilderImpl()
        {
            _resourceBundles = new List<ResourceBundle>();
            _inputProviders = new List<Tuple<Type, IEngineOptions>>();
            _extensions = new List<Tuple<Type, IEngineOptions>>();
            _loggers = new List<Tuple<Type, ILoggerOptions>>();
            _initScene = "init_scene";
        }

        public IGameBuilder AddResourceBundle(ResourceBundle bundle)
        {
            _resourceBundles.Add(bundle);
            return this;
        }

        public IGameBuilder UseScriptEngine<T, TOptions>(Action<TOptions> options = null)
            where T : class, IScriptEngine<TOptions>, new()
            where TOptions : class, IEngineOptions, new()
        {
            TOptions opts = new TOptions();
            options?.Invoke(opts);
            _scriptEngineOptions = opts;

            _scriptEngineType = typeof(T);
            return this;
        }

        public IGameBuilder UseRenderEngine<T, TOptions>(Action<TOptions> options = null)
            where T : class, IRenderEngine<TOptions>, new()
            where TOptions : class, IEngineOptions, new()
        {
            TOptions opts = new TOptions();
            options?.Invoke(opts);
            _renderEngineOptions = opts;

            _renderEngineType = typeof(T);
            return this;
        }

        public IGameBuilder UseSoundEngine<T, TOptions>(Action<TOptions> options = null)
            where T : class, ISoundEngine<TOptions>, new()
            where TOptions : class, IEngineOptions, new()
        {
            TOptions opts = new TOptions();
            options?.Invoke(opts);
            _soundEngineOptions = opts;

            _soundEngineType = typeof(T);
            return this;
        }

        public IGameBuilder UsePhysicsEngine<T, TOptions>(Action<TOptions> options = null)
            where T : class, IPhysicsEngine<TOptions>, new()
            where TOptions : class, IEngineOptions, new()
        {
            TOptions opts = new TOptions();
            options?.Invoke(opts);
            _physicsEngineOptions = opts;

            _physicsEngineType = typeof(T);
            return this;
        }

        public IGameBuilder AddInputProvider<T, TOptions>(Action<TOptions> options = null)
            where T : class, IInputProvider<TOptions>, new()
            where TOptions : class, IEngineOptions, new()
        {
            TOptions opts = new TOptions();
            options?.Invoke(opts);

            _inputProviders.Add(new Tuple<Type, IEngineOptions>(typeof(T), opts));
            return this;
        }

        public IGameBuilder AddExtension<T, TOptions>(Action<TOptions> options = null)
            where T : class, IEngineExtension<TOptions>, new()
            where TOptions : class, IEngineOptions, new()
        {
            TOptions opts = new TOptions();
            options?.Invoke(opts);

            _extensions.Add(new Tuple<Type, IEngineOptions>(typeof(T), opts));
            return this;
        }

        public IGameBuilder AddLogger<T, TOptions>(Action<TOptions> options = null)
            where T : class, ILogger<TOptions>, new()
            where TOptions : class, ILoggerOptions, new()
        {
            TOptions opts = new TOptions();
            options?.Invoke(opts);

            _loggers.Add(new Tuple<Type, ILoggerOptions>(typeof(T), opts));
            return this;
        }

        public IGameBuilder WithInitScene(string sceneId)
        {
            _initScene = sceneId;
            return this;
        }

        public IGameBuilderOutput Build()
        {
            return new GameBuilderOutputImpl
            {
                ResourceBundles = _resourceBundles,
                ScriptEngineType = _scriptEngineType,
                ScriptEngineOptions = _scriptEngineOptions,
                RenderEngineType = _renderEngineType,
                RenderEngineOptions = _renderEngineOptions,
                SoundEngineType = _soundEngineType,
                SoundEngineOptions = _soundEngineOptions,
                PhysicsEngineType = _physicsEngineType,
                PhysicsEngineOptions = _physicsEngineOptions,
                InputProviders = _inputProviders,
                Extensions = _extensions,
                Loggers = _loggers,
                InitScene = _initScene
            };
        }
    }
}