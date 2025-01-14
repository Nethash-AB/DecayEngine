using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
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
    public static class GameEngine
    {
        private static IGameEngine _instance;
        private static readonly object LockObject;

        static GameEngine()
        {
            LockObject = new object();
        }

        public static Task Init<TEngine>(Func<IGameBuilder, IGameBuilderOutput> gameBuilderFunction) where TEngine : class, IGameEngine, new()
        {
            if (_instance != null)
            {
                throw new Exception("DecayEngine already initialized.");
            }

            lock (LockObject)
            {
                _instance = new TEngine();
            }

            _instance.Init(gameBuilderFunction(_instance.GameBuilder)).Wait();

            return Task.Run(() =>
            {
                while (_instance.Alive) // What the fuck CoreRT, why are you ending execution when the entry thread ends but there are still threads running???????
                {
                    Task.Delay(500).Wait();
                }
            });
        }

        public static Task Shutdown()
        {
            if (_instance == null)
            {
                throw new Exception("DecayEngine not yet initialized.");
            }

            return _instance.Shutdown();
        }

        public static event Action<IScene, bool> OnScenePreload
        {
            add => GetInstance().OnScenePreload += value;
            remove => GetInstance().OnScenePreload -= value;
        }

        public static event Action<IScene, bool> OnScenePostload
        {
            add => GetInstance().OnScenePostload += value;
            remove => GetInstance().OnScenePostload -= value;
        }

        public static IEnumerable<ResourceBundle> ResourceBundles => GetInstance().ResourceBundles;
        public static IScriptEngine ScriptEngine => GetInstance().ScriptEngine;
        public static IRenderEngine RenderEngine => GetInstance().RenderEngine;
        public static ISoundEngine SoundEngine => GetInstance().SoundEngine;
        public static IPhysicsEngine PhysicsEngine => GetInstance().PhysicsEngine;
        public static IEnumerable<IInputProvider> InputProviders => GetInstance().InputProviders;
        public static IEnumerable<IScene> Scenes => GetInstance().Scenes;
        public static IEnumerable<IRenderUpdateable> Renderables => GetInstance().Renderables;
        public static IEnumerable<IScriptUpdateable> Updateables => GetInstance().Updateables;
        public static IReadOnlyDictionary<string, IInputAction> ActionMap => GetInstance().ActionMap;
        public static IScene ActiveScene => GetInstance().ActiveScene;
        public static TimeSpan EngineTime => GetInstance().EngineTime;

        public static IGameObject CreateGameObject(string prefabId, string name = null, IChildBearer<IGameObject> parent = null)
            => GetInstance().CreateGameObject(prefabId, name, parent);
        public static IGameObject CreateGameObject(PrefabResource prefab = null, string name = null, IChildBearer<IGameObject> parent = null)
            => GetInstance().CreateGameObject(prefab, name, parent);
        public static IInputAction CreateInputAction(string name) =>
            GetInstance().CreateInputAction(name);
        public static void RemoveInputAction(string name) =>
            GetInstance().RemoveInputAction(name);
        public static void ChangeScene(IScene scene)
            => GetInstance().ChangeScene(scene);
        public static TComponent GetComponent<TComponent>(IGameObject gameObject = null, Func<TComponent, bool> predicate = null)
            where TComponent : class, IComponent
            => GetInstance().GetComponent(gameObject, predicate);
        public static IEnumerable<TComponent> GetComponents<TComponent>(IGameObject gameObject = null, Func<TComponent, bool> predicate = null)
            where TComponent : class, IComponent
            => GetInstance().GetComponents(gameObject, predicate);
        public static IEnumerable<IComponent> GetComponents(IGameObject gameObject = null, Func<IComponent, bool> predicate = null)
            => GetInstance().GetComponents(gameObject, predicate);
        public static TComponent CreateComponent<TComponent>()
            where TComponent : class, IComponent
            => GetInstance().CreateComponent<TComponent>();
        public static IComponent CreateComponent(IRootResource resource)
            => GetInstance().CreateComponent(resource);
        public static TComponent CreateComponent<TComponent>(string resourceId)
            where TComponent : class, IComponent
            => GetInstance().CreateComponent<TComponent>(resourceId);
        public static TExtension GetExtension<TExtension>(Func<TExtension, bool> predicate = null)
            where TExtension : IEngineExtension
            => GetInstance().GetExtension(predicate);
        public static IEnumerable<TExtension> GetExtensions<TExtension>(Func<TExtension, bool> predicate = null)
            where TExtension : IEngineExtension
            => GetInstance().GetExtensions(predicate);
        public static ICoroutine CreateCoroutine(CoroutineDelegate coroutineFunc, CoroutineContext context = CoroutineContext.Script)
            => GetInstance().CreateCoroutine(coroutineFunc, context);
        public static ICoroutine<TState> CreateCoroutine<TState>(CoroutineDelegate<TState> coroutineFunc, ByReference<TState> state,
            CoroutineContext context = CoroutineContext.Script)
            => GetInstance().CreateCoroutine(coroutineFunc, state, context);
        public static void LogAppendLine(LogSeverity severity, string nameSpace, string message,
            [CallerLineNumber] int lineNumber = 0, [CallerMemberName]string callerName = "", [CallerFilePath] string sourcePath = "")
            => GetInstance().LogAppendLine(severity, nameSpace, message, lineNumber, callerName, sourcePath);

        private static IGameEngine GetInstance()
        {
            lock (LockObject)
            {
                if (_instance == null)
                {
                    throw new Exception("DecayEngine not yet initialized.");
                }
                return _instance;
            }
        }
    }
}