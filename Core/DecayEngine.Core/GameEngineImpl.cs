using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using DecayEngine.Core.Builder;
using DecayEngine.Core.Component;
using DecayEngine.Core.Input;
using DecayEngine.Core.Object;
using DecayEngine.DecPakLib;
using DecayEngine.DecPakLib.Bundle;
using DecayEngine.DecPakLib.Resource.Expression.Statement;
using DecayEngine.DecPakLib.Resource.Expression.Statement.Component;
using DecayEngine.DecPakLib.Resource.Expression.Statement.GameObject;
using DecayEngine.DecPakLib.Resource.RootElement;
using DecayEngine.DecPakLib.Resource.RootElement.Prefab;
using DecayEngine.DecPakLib.Resource.RootElement.Scene;
using DecayEngine.ModuleSDK;
using DecayEngine.ModuleSDK.Builder;
using DecayEngine.ModuleSDK.Capability;
using DecayEngine.ModuleSDK.Component;
using DecayEngine.ModuleSDK.Component.Script;
using DecayEngine.ModuleSDK.Coroutines;
using DecayEngine.ModuleSDK.Engine;
using DecayEngine.ModuleSDK.Engine.Extension;
using DecayEngine.ModuleSDK.Engine.Input;
using DecayEngine.ModuleSDK.Engine.Physics;
using DecayEngine.ModuleSDK.Engine.Render;
using DecayEngine.ModuleSDK.Engine.Script;
using DecayEngine.ModuleSDK.Engine.Sound;
using DecayEngine.ModuleSDK.Exports;
using DecayEngine.ModuleSDK.Exports.BaseExports.AnimatedMaterial;
using DecayEngine.ModuleSDK.Exports.BaseExports.AnimatedSprite;
using DecayEngine.ModuleSDK.Exports.BaseExports.Camera;
using DecayEngine.ModuleSDK.Exports.BaseExports.Coroutine;
using DecayEngine.ModuleSDK.Exports.BaseExports.GameObject;
using DecayEngine.ModuleSDK.Exports.BaseExports.Input;
using DecayEngine.ModuleSDK.Exports.BaseExports.Input.Triggers.GamePad;
using DecayEngine.ModuleSDK.Exports.BaseExports.Input.Triggers.Keyboard;
using DecayEngine.ModuleSDK.Exports.BaseExports.Scene;
using DecayEngine.ModuleSDK.Exports.BaseExports.SceneController;
using DecayEngine.ModuleSDK.Exports.BaseExports.Script;
using DecayEngine.ModuleSDK.Exports.BaseExports.Shader;
using DecayEngine.ModuleSDK.Exports.BaseExports.ShaderProgram;
using DecayEngine.ModuleSDK.Exports.BaseExports.Sound;
using DecayEngine.ModuleSDK.Exports.BaseExports.SoundBank;
using DecayEngine.ModuleSDK.Exports.BaseExports.Texture;
using DecayEngine.ModuleSDK.Exports.BaseExports.Transform;
using DecayEngine.ModuleSDK.Expression;
using DecayEngine.ModuleSDK.Logging;
using DecayEngine.ModuleSDK.Logging.Exports;
using DecayEngine.ModuleSDK.Math.Exports;
using DecayEngine.ModuleSDK.Object.GameObject;
using DecayEngine.ModuleSDK.Object.GameStructure;
using DecayEngine.ModuleSDK.Object.Input;
using DecayEngine.ModuleSDK.Object.Input.Triggers.GamePad;
using DecayEngine.ModuleSDK.Object.Input.Triggers.Keyboard;
using DecayEngine.ModuleSDK.Object.Scene;
using DecayEngine.StubEngines;
using DecayEngine.StubEngines.Physics;
using DecayEngine.StubEngines.Render;
using DecayEngine.StubEngines.Script;
using DecayEngine.StubEngines.Sound;

namespace DecayEngine.Core
{
    public class GameEngineImpl : IGameEngine
    {
        public IGameBuilder GameBuilder => new GameBuilderImpl();
        public List<ResourceBundle> ResourceBundles { get; }
        public IScriptEngine ScriptEngine { get; private set; }
        public IRenderEngine RenderEngine { get; private set; }
        public ISoundEngine SoundEngine { get; private set; }
        public IPhysicsEngine PhysicsEngine { get; private set; }
        public IEnumerable<IInputProvider> InputProviders => _inputProviders;

        public IEnumerable<IScene> Scenes => _scenes;

        public IEnumerable<IRenderUpdateable> Renderables
        {
            get
            {
                _renderables.RemoveAll(x => x == null || x is IDestroyable destroyable && destroyable.Destroyed);
                return _renderables;
            }
        }

        public IEnumerable<IScriptUpdateable> Updateables
        {
            get
            {
                _updateables.RemoveAll(x => x == null || x is IDestroyable destroyable && destroyable.Destroyed);
                return _updateables;
            }
        }

        public IReadOnlyDictionary<string, IInputAction> ActionMap => _actionMap;

        public IScene ActiveScene { get; private set; }

        public bool Alive => _mainThread.IsAlive;
        public TimeSpan EngineTime => _stopwatch.Elapsed;

        public event Action<IScene, bool> OnScenePreload;
        public event Action<IScene, bool> OnScenePostload;

        private readonly object _lockObject;
        private readonly object _mainThreadStackLockObject;
        private readonly List<IScene> _scenes;
        private readonly List<IRenderUpdateable> _renderables;
        private readonly List<IScriptUpdateable> _updateables;
        private readonly List<IComponentFactory> _componentFactories;
        private readonly List<IEngineExtension> _extensions;
        private readonly List<IInputProvider> _inputProviders;
        private readonly List<ILogger> _loggers;

        private readonly Dictionary<string, IInputAction> _actionMap;

        private Thread _mainThread;
        private Stack<Task> _mainThreadActionStack;
        private bool _running;
        private Stopwatch _stopwatch;

        public GameEngineImpl()
        {
            _lockObject = new object();
            _mainThreadStackLockObject = new object();

            ResourceBundles = new List<ResourceBundle>();
            _renderables = new List<IRenderUpdateable>();
            _updateables = new List<IScriptUpdateable>();
            _scenes = new List<IScene>();
            _componentFactories = new List<IComponentFactory>();
            _extensions = new List<IEngineExtension>();
            _inputProviders = new List<IInputProvider>();
            _loggers = new List<ILogger>();

            _actionMap = new Dictionary<string, IInputAction>();

            _stopwatch = new Stopwatch();
        }

        public Task Init(IGameBuilderOutput output)
        {
            // Thread Init
            _mainThread = new Thread(Loop)
            {
                IsBackground = false,
                Priority = ThreadPriority.Lowest,
            };
            _mainThread.Start();

            while (!_running)
            {
                Task.Delay(1);
            }

            Task coreInitTask = new Task(async () =>
            {
                _stopwatch.Start();

                ResourceBundles.AddRange(output.ResourceBundles);

                const string initMethodName = nameof(IEngine<StubEngineOptions>.Init);

                foreach ((Type loggerType, ILoggerOptions loggerOptions) in output.Loggers)
                {
                    ILogger logger = (ILogger) Activator.CreateInstance(loggerType);
                    _loggers.Add(logger);

                    MethodInfo initMethod = loggerType.GetMethod(initMethodName, new[] {loggerOptions.GetType()});
                    if (initMethod == null) continue;

                    Task initTask = (Task) initMethod.Invoke(logger, new object[] {loggerOptions});
                    await initTask;
                }

                // Script Engine
                Type scriptEngineType = output.ScriptEngineType;
                if (scriptEngineType == null)
                {
                    scriptEngineType = typeof(StubScriptEngine);
                }

                LogAppendLine(LogSeverity.Info, "Core", $"Initializing Script Engine: {scriptEngineType.Name}.",
                    158, "Init", "GameEngineImpl.cs");
                ScriptEngine = (IScriptEngine) Activator.CreateInstance(scriptEngineType);
                _componentFactories.AddRange(ScriptEngine.ComponentFactories);
                Task scriptInitTask = (Task) scriptEngineType
                    .GetMethod(initMethodName, new[] {output.ScriptEngineOptions.GetType()})?
                    .Invoke(ScriptEngine, new object[] {output.ScriptEngineOptions});
                if (scriptInitTask != null)
                {
                    await scriptInitTask;
                    await ScriptEngine.InjectExports(ScriptEngine.ScriptExports);
                    await ScriptEngine.InjectExports(GenerateDefaultExports());
                }

                // Render Engine
                Type renderEngineType = output.RenderEngineType;
                if (renderEngineType == null)
                {
                    renderEngineType = typeof(StubRenderEngine);
                }

                LogAppendLine(LogSeverity.Info, "Core", $"Initializing Render Engine: {renderEngineType.Name}.",
                    179, "Init", "GameEngineImpl.cs");
                RenderEngine = (IRenderEngine) Activator.CreateInstance(renderEngineType);
                await ScriptEngine.InjectExports(RenderEngine.ScriptExports);
                _componentFactories.AddRange(RenderEngine.ComponentFactories);
                Task renderInitTask = (Task) renderEngineType
                    .GetMethod(initMethodName, new[] {output.RenderEngineOptions.GetType()})?
                    .Invoke(RenderEngine, new object[] {output.RenderEngineOptions});
                if (renderInitTask != null)
                {
                    await renderInitTask;
                }

                // Sound Engine
                Type soundEngineType = output.SoundEngineType;
                if (soundEngineType == null)
                {
                    soundEngineType = typeof(StubSoundEngine);
                }

                LogAppendLine(LogSeverity.Info, "Core", $"Initializing Sound Engine: {soundEngineType.Name}.",
                    199, "Init", "GameEngineImpl.cs");
                SoundEngine = (ISoundEngine) Activator.CreateInstance(soundEngineType);
                await ScriptEngine.InjectExports(SoundEngine.ScriptExports);
                _componentFactories.AddRange(SoundEngine.ComponentFactories);
                Task soundInitTask = (Task) soundEngineType
                    .GetMethod(initMethodName, new[] {output.SoundEngineOptions.GetType()})?
                    .Invoke(SoundEngine, new object[] {output.SoundEngineOptions});
                if (soundInitTask != null)
                {
                    await soundInitTask;
                }

                // Physics Engine
                Type physicsEngineType = output.PhysicsEngineType;
                if (physicsEngineType == null)
                {
                    physicsEngineType = typeof(StubPhysicsEngine);
                }

                LogAppendLine(LogSeverity.Info, "Core", $"Initializing Physics Engine: {physicsEngineType.Name}.",
                    219, "Init", "GameEngineImpl.cs");
                PhysicsEngine = (IPhysicsEngine) Activator.CreateInstance(physicsEngineType);
                await ScriptEngine.InjectExports(PhysicsEngine.ScriptExports);
                _componentFactories.AddRange(PhysicsEngine.ComponentFactories);
                Task physicsInitTask = (Task) physicsEngineType
                    .GetMethod(initMethodName, new[] {output.PhysicsEngineOptions.GetType()})?
                    .Invoke(PhysicsEngine, new object[] {output.PhysicsEngineOptions});
                if (physicsInitTask != null)
                {
                    await physicsInitTask;
                }

                // Engine Extensions
                LogAppendLine(LogSeverity.Info, "Core", "Initializing Extensions.",
                    233, "Init", "GameEngineImpl.cs");
                foreach ((Type extensionType, IEngineOptions extensionOptions) in output.Extensions)
                {
                    if (_extensions.Any(ext => ext.GetType() == extensionType))
                    {
                        LogAppendLine(LogSeverity.Info, "Core", $"Skipping duplicate Extension: {extensionType}",
                            239, "Init", "GameEngineImpl.cs");
                        continue;
                    }

                    LogAppendLine(LogSeverity.Info, "Core", $"Initializing Extension: {extensionType.Name}.",
                        244, "Init", "GameEngineImpl.cs");
                    IEngineExtension extension = (IEngineExtension) Activator.CreateInstance(extensionType);
                    await ScriptEngine.InjectExports(extension.ScriptExports);
                    _extensions.Add(extension);
                    _componentFactories.AddRange(extension.ComponentFactories);

                    MethodInfo initMethod = extensionType.GetMethod(initMethodName, new[] {extensionOptions.GetType()});
                    if (initMethod == null) continue;

                    Task initTask = (Task) initMethod.Invoke(extension, new object[] {extensionOptions});
                    await initTask;
                }

                // Input Providers
                foreach ((Type ipType, IEngineOptions ipOptions) in output.InputProviders)
                {
                    if (_inputProviders.Any(ip => ip.GetType() == ipType))
                    {
                        LogAppendLine(LogSeverity.Info, "Core", $"Skipping duplicate Input Provider: {ipType}",
                            239, "Init", "GameEngineImpl.cs");
                        continue;
                    }

                    LogAppendLine(LogSeverity.Info, "Core", $"Initializing Input Provider: {ipType.Name}.",
                        244, "Init", "GameEngineImpl.cs");
                    IInputProvider inputProvider = (IInputProvider) Activator.CreateInstance(ipType);
                    await ScriptEngine.InjectExports(inputProvider.ScriptExports);
                    _inputProviders.Add(inputProvider);
                    _componentFactories.AddRange(inputProvider.ComponentFactories);

                    MethodInfo initMethod = ipType.GetMethod(initMethodName, new[] {ipOptions.GetType()});
                    if (initMethod == null) continue;

                    Task initTask = (Task) initMethod.Invoke(inputProvider, new object[] {ipOptions});
                    await initTask;
                }

                // Scene Init
                Scene initScene = null;
                foreach (ResourceBundle bundle in GameEngine.ResourceBundles)
                {
                    foreach (SceneResource sceneResource in bundle.Resources.OfType<SceneResource>())
                    {
                        Scene scene = new Scene(sceneResource);
                        _scenes.Add(scene);
                        if (sceneResource.Id == output.InitScene)
                        {
                            initScene = scene;
                        }
                    }
                }

                if (initScene != null)
                {
                    LogAppendLine(LogSeverity.Info, "Core", $"Loading Init Scene: {initScene.Name}",
                        275, "Init", "GameEngineImpl.cs");
                    ChangeScene(initScene, true);
                    LogAppendLine(LogSeverity.Info, "Core", $"Finished Loading Init Scene: {initScene.Name}",
                        278, "Init", "GameEngineImpl.cs");
                }
                else
                {
                    throw new Exception($"Failed to load init scene. No scene found with id {output.InitScene}.");
                }
            });

            _mainThreadActionStack.Push(coreInitTask);
            return coreInitTask;
        }

        public Task Shutdown()
        {
            Task shutdownTask = new Task(() =>
            {
                ScriptEngine.Shutdown().Wait();
                RenderEngine.Shutdown().Wait();
                SoundEngine.Shutdown().Wait();
                PhysicsEngine.Shutdown().Wait();

                // ReSharper disable once InconsistentlySynchronizedField
                foreach (IEngineExtension engineExtension in _extensions)
                {
                    engineExtension.Shutdown().Wait();
                }

                foreach (IInputProvider inputProvider in _inputProviders)
                {
                    inputProvider.Shutdown().Wait();
                }

                lock (_lockObject)
                {
                    _scenes.Clear();
                    _componentFactories.Clear();
                    ResourceBundles.Clear();
                    _extensions.Clear();
                    _inputProviders.Clear();
                }

                _stopwatch.Stop();
                _running = false;

                GameEngine.LogAppendLine(LogSeverity.Info, "Core", "Decay Engine terminated.");
            });

            _mainThreadActionStack.Push(shutdownTask);
            return shutdownTask;
        }

        public IGameObject CreateGameObject(string prefabId, string name = null, IChildBearer<IGameObject> parent = null)
        {
            // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
            foreach (ResourceBundle resourceBundle in ResourceBundles)
            {
                foreach (PrefabResource prefab in resourceBundle.Resources.OfType<PrefabResource>())
                {
                    if (prefab.Id != prefabId) continue;
                    return CreateGameObject(prefab, name, parent);
                }
            }

            throw new ArgumentException($"No prefab with id {prefabId} found.");
        }

        public IGameObject CreateGameObject(PrefabResource prefab = null, string name = null, IChildBearer<IGameObject> parent = null)
        {
            GameObject go = new GameObject
            {
                Name = name
            };

            if (prefab == null)
            {
                if (parent != null)
                {
                    parent.AddChild(go);
                }
                else
                {
                    ActiveScene.AddChild(go);
                }
                return go;
            }

            go.Resource = prefab;
            if (parent != null)
            {
                parent.AddChild(go);
            }
            else
            {
                ActiveScene.AddChild(go);
            }

            List<Tuple<IActivable, bool>> activables = new List<Tuple<IActivable, bool>>();

            foreach (CreateComponentExpression childComponentExpression in prefab.Children.OfType<CreateComponentExpression>())
            {
                StatementExpressionResolver resolver = new StatementExpressionResolver(childComponentExpression);
                object result = resolver.Resolve((IGameStructure) go);
                if (!(result is IActivable activable)) continue;

                activables.Add(new Tuple<IActivable, bool>(activable, childComponentExpression.Active));
            }

            foreach (CreateGameObjectExpression childGameObjectExpression in prefab.Children.OfType<CreateGameObjectExpression>())
            {
                StatementExpressionResolver resolver = new StatementExpressionResolver(childGameObjectExpression);
                object result = resolver.Resolve((IGameStructure) go);
                if (!(result is IActivable activable)) continue;

                activables.Add(new Tuple<IActivable, bool>(activable, childGameObjectExpression.Active));
            }

            foreach ((IActivable activable, bool value) in activables.Where(a => a.Item1 is IGameObject))
            {
                activable.Active = value;
            }

            foreach ((IActivable activable, bool value) in activables.Where(a => !(a.Item1 is IGameObject)))
            {
                activable.Active = value;
            }

            return go;
        }

        public void ChangeScene(IScene scene)
        {
            ChangeScene(scene, false);
        }

        public IInputAction CreateInputAction(string name)
        {
            if (_actionMap.ContainsKey(name))
            {
                return _actionMap[name];
            }

            return _actionMap[name] = new InputAction();
        }

        public void RemoveInputAction(string name)
        {
            if (_actionMap.ContainsKey(name))
            {
                _actionMap.Remove(name);
            }
        }

        private void ChangeScene(IScene scene, bool isInit)
        {
            if (ActiveScene != null)
            {
                foreach (IGameObject child in ActiveScene.Children)
                {
                    if (child.Persistent)
                    {
                        child.SetParent(scene);
                    }
                    else
                    {
                        child.Destroy();
                    }
                }

                foreach (ISceneAttachableComponent component in ActiveScene.Components)
                {
                    if (component.Persistent)
                    {
                        scene.AttachComponent(component);
                    }
                    else
                    {
                        component.Destroy();
                    }
                }
            }

            ActiveScene = scene;

            OnScenePreload?.Invoke(scene, isInit);

            List<Tuple<IActivable, bool>> activables = new List<Tuple<IActivable, bool>>();
            if (ActiveScene.Resource.Children != null)
            {
                foreach (IStatementExpression childExpression in ActiveScene.Resource.Children)
                {
                    StatementExpressionResolver resolver = new StatementExpressionResolver(childExpression);
                    object result = resolver.Resolve((IGameStructure) ActiveScene);
                    if (!(result is IActivable activable)) continue;

                    switch (childExpression)
                    {
                        case CreateComponentExpression createComponentExpression:
                            activables.Add(new Tuple<IActivable, bool>(activable, createComponentExpression.Active));
                            break;
                        case CreateGameObjectExpression createGameObjectExpression:
                            activables.Add(new Tuple<IActivable, bool>(activable, createGameObjectExpression.Active));
                            break;
                    }
                }
            }

            List<IScript> autoInitScripts = new List<IScript>();

            List<Tuple<IActivable, bool>> sceneActivables =
                ActiveScene.Components.SelectMany(component => activables.Where(pair => pair.Item1 == component)).ToList();
            foreach ((IActivable activable, bool value) in sceneActivables)
            {
                activable.Active = value;
                if (value && activable is IScript script)
                {
                    autoInitScripts.Add(script);
                }
                activables.RemoveAll(pair => pair.Item1 == activable);
            }
            foreach (IScript script in autoInitScripts)
            {
                script.OnInit().Wait();
            }
            autoInitScripts.Clear();

            foreach ((IActivable activable, bool value) in activables)
            {
                activable.Active = value;
                if (value && activable is IScript script)
                {
                    autoInitScripts.Add(script);
                }
            }
            foreach (IScript script in autoInitScripts)
            {
                script.OnInit().Wait();
            }

            OnScenePostload?.Invoke(ActiveScene, isInit);
        }

        public TComponent GetComponent<TComponent>(IGameObject gameObject = null, Func<TComponent, bool> predicate = null)
            where TComponent : class, IComponent
        {
            if (predicate == null)
            {
                predicate = component => true;
            }

            if (gameObject == null)
            {
                return ActiveScene.Children.Select(go => GetComponent(go, predicate)).FirstOrDefault(component => component != null);
            }

            foreach (IGameObject goChild in gameObject.Children)
            {
                TComponent component = goChild.Components.OfType<TComponent>().FirstOrDefault(predicate);
                if (component != null)
                {
                    return component;
                }

                foreach (IGameObject goChildChild in goChild.Children)
                {
                    component = GetComponent(goChildChild, predicate);
                    if (component != null)
                    {
                        return component;
                    }
                }
            }

            return null;
        }

        public IEnumerable<TComponent> GetComponents<TComponent>(IGameObject gameObject = null, Func<TComponent, bool> predicate = null)
            where TComponent : class, IComponent
        {
            if (predicate == null)
            {
                predicate = component => true;
            }

            if (gameObject == null)
            {
                return ActiveScene.Children.SelectMany(go => GetComponents(go, predicate)).ToList();
            }

            return gameObject.Components.OfType<TComponent>().Where(predicate)
                .Concat(gameObject.Children.SelectMany(go => GetComponents(go, predicate))).ToList();
        }

        public IEnumerable<IComponent> GetComponents(IGameObject gameObject = null, Func<IComponent, bool> predicate = null)
        {
            return GetComponents<IComponent>(gameObject, predicate);
        }

        public TComponent CreateComponent<TComponent>()
            where TComponent : class, IComponent
        {
            TComponent component = InvokeComponentCreationMethod<TComponent>();
            return component;
        }

        public IComponent CreateComponent(IRootResource resource)
        {
            IComponent component = InvokeComponentCreationMethod<IComponent>(resource);

            if (component is IRenderUpdateable renderable)
            {
                _renderables.Add(renderable);
            }

            if (component is IScriptUpdateable updateable)
            {
                _updateables.Add(updateable);
            }

            return component;
        }

        public TComponent CreateComponent<TComponent>(string resourceId)
            where TComponent : class, IComponent
        {
            Type genericComponentInterface = typeof(TComponent).GetInterface(typeof(IComponent<>).Name, true);
            if (genericComponentInterface == null)
            {
                throw new Exception($"Component of type {typeof(TComponent)} is not a resource based component.");
            }
            Type resourceType = genericComponentInterface.GetGenericArguments().FirstOrDefault(type => typeof(IRootResource).IsAssignableFrom(type));

            IRootResource resource = null;
            foreach (ResourceBundle resourceBundle in ResourceBundles)
            {
                foreach (IRootResource res in resourceBundle.Resources.OfType<IRootResource>())
                {
                    if (res.Id != resourceId || res.GetType() != resourceType) continue;
                    resource = res;
                    break;
                }
            }

            if (resource == null)
            {
                throw new ArgumentException($"No resource of type {resourceType} with id {resourceId} found.");
            }

            TComponent component = InvokeComponentCreationMethod<TComponent>(resource);

            if (component is IRenderUpdateable renderable)
            {
                _renderables.Add(renderable);
            }

            if (component is IScriptUpdateable updateable)
            {
                _updateables.Add(updateable);
            }

            return component;
        }

        public TExtension GetExtension<TExtension>(Func<TExtension, bool> predicate = null)
            where TExtension : IEngineExtension
        {
            if (predicate == null)
            {
                predicate = extension => true;
            }

            return GetExtensions(predicate).FirstOrDefault();
        }

        public IEnumerable<TExtension> GetExtensions<TExtension>(Func<TExtension, bool> predicate = null)
            where TExtension : IEngineExtension
        {
            if (predicate == null)
            {
                predicate = extension => true;
            }

            List<TExtension> extensions;
            lock (_lockObject)
            {
                extensions = _extensions.OfType<TExtension>().Where(predicate).ToList();
            }

            return extensions;
        }

        public ICoroutine CreateCoroutine(CoroutineDelegate body, CoroutineContext context)
        {
            return context switch
            {
                CoroutineContext.Script => ScriptEngine.EngineThread.CreateCoroutine(body),
                CoroutineContext.Render => RenderEngine.EngineThread.CreateCoroutine(body),
                CoroutineContext.Physics => PhysicsEngine.EngineThread.CreateCoroutine(body),
                CoroutineContext.Sound => SoundEngine.EngineThread.CreateCoroutine(body),
                _ => null
            };
        }

        public ICoroutine<TState> CreateCoroutine<TState>(CoroutineDelegate<TState> body, ByReference<TState> state, CoroutineContext context)
        {
            return context switch
            {
                CoroutineContext.Script => ScriptEngine.EngineThread.CreateCoroutine(body, state),
                CoroutineContext.Render => RenderEngine.EngineThread.CreateCoroutine(body, state),
                CoroutineContext.Physics => PhysicsEngine.EngineThread.CreateCoroutine(body, state),
                CoroutineContext.Sound => SoundEngine.EngineThread.CreateCoroutine(body, state),
                _ => null
            };
        }

        public void LogAppendLine(LogSeverity severity, string nameSpace, string message, int lineNumber, string callerName, string sourcePath)
        {
            foreach (ILogger logger in _loggers.Where(logger => severity >= logger.MinimumSeverity))
            {
                logger.AppendLine(severity, nameSpace, message, lineNumber, callerName, sourcePath);
            }
        }

        private TComponent InvokeComponentCreationMethod<TComponent>(IRootResource resource = null, params object[] args)
            where TComponent : class, IComponent
        {
            Type expectedFactoryType;
            if (resource != null)
            {
                object[] newArgs = new object[args.Length + 1];
                newArgs[0] = resource;
                Array.Copy(args, 0, newArgs, 1, args.Length);
                args = newArgs;

                expectedFactoryType = typeof(IComponentFactory<,>).MakeGenericType(typeof(TComponent), resource.GetType());
            }
            else
            {
                expectedFactoryType = typeof(IComponentFactory<TComponent>);
            }

            var factoryData = (from factory in _componentFactories
                from iface in factory.GetType().GetInterfaces()
                where iface.IsGenericType &&
                      iface.GetGenericArguments().SequenceEqual(expectedFactoryType.GetGenericArguments(), new FactoryTypeEqualityComparer())
                select new {Factory = factory, Iface = iface}).FirstOrDefault();

            if (factoryData == null)
            {
                if (typeof(TComponent) == typeof(IComponent) && resource != null)
                {
                    throw new ArgumentException($"No component factory available for resource type {resource.GetType().Name}");
                }
                else
                {
                    throw new ArgumentException($"No component factory available for component type {typeof(TComponent).Name}");
                }
            }

            const string createComponentMethodName = nameof(IComponentFactory<IComponent>.CreateComponent);

            MethodInfo createComponentMethod = factoryData.Iface
                .GetMethods(BindingFlags.Instance | BindingFlags.Public)
                .FirstOrDefault(method => method.Name == createComponentMethodName);

            try
            {
                return (TComponent) createComponentMethod?.Invoke(factoryData.Factory, args);
            }
            catch (TargetInvocationException e)
            {
                if (resource != null)
                {
                    if (typeof(TComponent) == typeof(IComponent))
                    {
                        GameEngine.LogAppendLine(LogSeverity.Error, "Core",
                            $"Failed to invoke {createComponentMethodName} method on component factory {factoryData.Factory.GetType().Name} " +
                            $"for resource type {resource.GetType().Name}. See Inner Exception for more details.");
                    }
                    else
                    {
                        GameEngine.LogAppendLine(LogSeverity.Error, "Core",
                            $"Failed to invoke {createComponentMethodName} method on component factory {factoryData.Factory.GetType().Name} " +
                            $"for component type {typeof(TComponent).Name} and resource type {resource.GetType().Name}. See Inner Exception for more details.");
                    }
                }
                else
                {
                    GameEngine.LogAppendLine(LogSeverity.Error, "Core",
                        $"Failed to invoke {createComponentMethodName} method on component factory {factoryData.Factory.GetType().Name} " +
                        $"for component type {typeof(TComponent).Name}. See Inner Exception for more details.");
                }

                if (e.InnerException != null)
                {
                    throw e.InnerException;
                }
                throw;
            }
        }

        private static ScriptExports GenerateDefaultExports()
        {
            ScriptExports exports = new ScriptExports();

            // Base Exports
            exports.Types.Add(typeof(ManagedExportType));
            exports.Types.Add(typeof(EventExport<>));
            exports.Types.Add(typeof(Vector2Export));
            exports.Types.Add(typeof(Vector3Export));
            exports.Types.Add(typeof(Vector4Export));
            exports.Types.Add(typeof(QuaternionExport));
            exports.Types.Add(typeof(LogSeverity));
            exports.Types.Add(typeof(TransformExport));

            // Render Exports
            exports.Types.Add(typeof(CameraExport));
            exports.Types.Add(typeof(PerspectiveCameraExport));
            exports.Types.Add(typeof(OrthographicCameraExport));
            exports.Types.Add(typeof(ShaderExport));
            exports.Types.Add(typeof(ShaderProgramExport));
            exports.Types.Add(typeof(SoundPlaybackStatus));
            exports.Types.Add(typeof(SoundExport));
            exports.Types.Add(typeof(SoundBankExport));
            exports.Types.Add(typeof(AnimatedSpriteExport));
            exports.Types.Add(typeof(TextureExport));
            exports.Types.Add(typeof(AnimatedMaterialExport));

            // Script Exports
            exports.Types.Add(typeof(ScriptExport));

            // Scene Exports
            exports.Types.Add(typeof(SceneExport));
            exports.Types.Add(typeof(SceneControllerExport));

            // GameObject Exports
            exports.Types.Add(typeof(GameObjectExport));

            // Coroutine Exports
            exports.Types.Add(typeof(CoroutineContextExport));

            // Input Exports
            exports.Types.Add(typeof(InputControllerExport));
            exports.Types.Add(typeof(GamePadAxisScanCode));
            exports.Types.Add(typeof(GamePadButtonScanCode));
            exports.Types.Add(typeof(KeyboardScanCode));
            exports.Types.Add(typeof(InputActionTriggerExport));
            exports.Types.Add(typeof(InputActionTriggerDigitalExport));
            exports.Types.Add(typeof(InputActionTriggerAnalogExport));
            exports.Types.Add(typeof(GamePadButtonActionTriggerExport));
            exports.Types.Add(typeof(GamePadAxisActionTriggerExport));
            exports.Types.Add(typeof(KeyboardActionTriggerExport));
            exports.Types.Add(typeof(InputActionExport));

            // Logging Exports
            exports.Functions.Add(typeof(LoggingNamespaceExport).GetMethod(nameof(LoggingNamespaceExport.Append)));
            exports.Functions.Add(typeof(ConsoleNamespaceExport).GetMethod(nameof(ConsoleNamespaceExport.log)));

            return exports;
        }

        private void Loop()
        {
            lock (_mainThreadStackLockObject)
            {
                _mainThreadActionStack = new Stack<Task>();
            }

            GameEngine.LogAppendLine(LogSeverity.Info, "Core", $"Decay Engine loaded. Thread ID: {_mainThread.ManagedThreadId}");

            _running = true;
            while (_running)
            {
                RunStackedTasks();
            }
        }

        private void RunStackedTasks()
        {
            lock (_mainThreadStackLockObject)
            {
                while (_mainThreadActionStack.Count > 0)
                {
                    Task task = _mainThreadActionStack.Pop();
                    if (task == null || task.Status != TaskStatus.Created) continue;

                    try
                    {
                        task.RunSynchronously();
                    }
                    catch (InvalidOperationException)
                    {
                    }
                }
            }
        }
    }
}