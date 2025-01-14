using System;
using System.Collections.Generic;
using DecayEngine.DecPakLib.Bundle;
using DecayEngine.ModuleSDK.Engine;
using DecayEngine.ModuleSDK.Logging;

namespace DecayEngine.ModuleSDK.Builder
{
    public interface IGameBuilderOutput
    {
        IEnumerable<ResourceBundle> ResourceBundles { get; }

        Type ScriptEngineType { get; }
        IEngineOptions ScriptEngineOptions { get; }

        Type RenderEngineType { get; }
        IEngineOptions RenderEngineOptions { get; }

        Type SoundEngineType { get; }
        IEngineOptions SoundEngineOptions { get; }

        Type PhysicsEngineType { get; }
        IEngineOptions PhysicsEngineOptions { get; }

        IEnumerable<Tuple<Type, IEngineOptions>> InputProviders { get; }
        IEnumerable<Tuple<Type, IEngineOptions>> Extensions { get; }
        IEnumerable<Tuple<Type, ILoggerOptions>> Loggers { get; }
        string InitScene { get; }
    }
}