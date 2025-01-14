using System;
using System.Collections.Generic;
using DecayEngine.DecPakLib.Bundle;
using DecayEngine.ModuleSDK.Builder;
using DecayEngine.ModuleSDK.Engine;
using DecayEngine.ModuleSDK.Logging;

namespace DecayEngine.Core.Builder
{
    public class GameBuilderOutputImpl : IGameBuilderOutput
    {
        public IEnumerable<ResourceBundle> ResourceBundles { get; internal set; }

        public Type ScriptEngineType { get; internal set; }
        public IEngineOptions ScriptEngineOptions { get; internal set; }

        public Type RenderEngineType { get; internal set; }
        public IEngineOptions RenderEngineOptions { get; internal set; }

        public Type SoundEngineType { get; internal set; }
        public IEngineOptions SoundEngineOptions { get; internal set; }

        public Type PhysicsEngineType { get; internal set; }
        public IEngineOptions PhysicsEngineOptions { get; internal set; }

        public IEnumerable<Tuple<Type, IEngineOptions>> InputProviders { get; internal set; }
        public IEnumerable<Tuple<Type, IEngineOptions>> Extensions { get; internal set; }
        public IEnumerable<Tuple<Type, ILoggerOptions>> Loggers { get; internal set; }
        public string InitScene { get; internal set; }
    }
}