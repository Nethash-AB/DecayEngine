using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using DecayEngine.DecPakLib;
using DecayEngine.DecPakLib.Bundle;
using DecayEngine.DecPakLib.Resource.RootElement;
using DecayEngine.DecPakLib.Resource.RootElement.Prefab;
using DecayEngine.ModuleSDK.Builder;
using DecayEngine.ModuleSDK.Capability;
using DecayEngine.ModuleSDK.Component;
using DecayEngine.ModuleSDK.Coroutines;
using DecayEngine.ModuleSDK.Engine.Extension;
using DecayEngine.ModuleSDK.Engine.Input;
using DecayEngine.ModuleSDK.Engine.Physics;
using DecayEngine.ModuleSDK.Engine.Render;
using DecayEngine.ModuleSDK.Engine.Script;
using DecayEngine.ModuleSDK.Engine.Sound;
using DecayEngine.ModuleSDK.Logging;
using DecayEngine.ModuleSDK.Object;
using DecayEngine.ModuleSDK.Object.GameObject;
using DecayEngine.ModuleSDK.Object.Input;
using DecayEngine.ModuleSDK.Object.Scene;

namespace DecayEngine.ModuleSDK
{
    public interface IGameEngine
    {
        List<ResourceBundle> ResourceBundles { get; }
        IScriptEngine ScriptEngine { get; }
        IRenderEngine RenderEngine { get; }
        ISoundEngine SoundEngine { get; }
        IPhysicsEngine PhysicsEngine { get; }
        IEnumerable<IInputProvider> InputProviders { get; }
        IEnumerable<IScene> Scenes { get; }
        IGameBuilder GameBuilder { get; }
        IEnumerable<IRenderUpdateable> Renderables { get; }
        IEnumerable<IScriptUpdateable> Updateables { get; }
        IReadOnlyDictionary<string, IInputAction> ActionMap { get; }
        IScene ActiveScene { get; }
        bool Alive { get; }
        TimeSpan EngineTime { get; }

        event Action<IScene, bool> OnScenePreload;
        event Action<IScene, bool> OnScenePostload;

        Task Init(IGameBuilderOutput output);
        Task Shutdown();
        IGameObject CreateGameObject(string prefabId, string name = null, IChildBearer<IGameObject> parent = null);
        IGameObject CreateGameObject(PrefabResource prefab = null, string name = null, IChildBearer<IGameObject> parent = null);
        IInputAction CreateInputAction(string name);
        void RemoveInputAction(string name);
        void ChangeScene(IScene scene);
        TComponent GetComponent<TComponent>(IGameObject gameObject = null, Func<TComponent, bool> predicate = null)
            where TComponent : class, IComponent;
        IEnumerable<TComponent> GetComponents<TComponent>(IGameObject gameObject = null, Func<TComponent, bool> predicate = null)
            where TComponent : class, IComponent;
        IEnumerable<IComponent> GetComponents(IGameObject gameObject = null, Func<IComponent, bool> predicate = null);
        TComponent CreateComponent<TComponent>()
            where TComponent : class, IComponent;
        IComponent CreateComponent(IRootResource resource);
        TComponent CreateComponent<TComponent>(string resourceId)
            where TComponent : class, IComponent;
        TExtension GetExtension<TExtension>(Func<TExtension, bool> predicate = null)
            where TExtension : IEngineExtension;
        IEnumerable<TExtension> GetExtensions<TExtension>(Func<TExtension, bool> predicate = null)
            where TExtension : IEngineExtension;
        ICoroutine CreateCoroutine(CoroutineDelegate body, CoroutineContext context = CoroutineContext.Script);
        ICoroutine<TState> CreateCoroutine<TState>(CoroutineDelegate<TState> body, ByReference<TState> state, CoroutineContext context = CoroutineContext.Script);
        void LogAppendLine(LogSeverity severity, string nameSpace, string message, int lineNumber, string callerName, string sourcePath);
    }
}