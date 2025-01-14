using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DecayEngine.DecPakLib;
using DecayEngine.DecPakLib.Resource.RootElement.Script;
using DecayEngine.DecPakLib.Resource.Structure.Component.Script;
using DecayEngine.ModuleSDK;
using DecayEngine.ModuleSDK.Capability;
using DecayEngine.ModuleSDK.Component.Script;
using DecayEngine.ModuleSDK.Exports.BaseExports.GameObject;
using DecayEngine.ModuleSDK.Exports.BaseExports.Script;
using DecayEngine.ModuleSDK.Expression;
using DecayEngine.ModuleSDK.Logging;
using DecayEngine.ModuleSDK.Object;
using DecayEngine.ModuleSDK.Object.GameObject;
using DecayEngine.ModuleSDK.Object.Scene;
using NiL.JS;
using NiL.JS.BaseLibrary;
using NiL.JS.Core;
using NiL.JS.Extensions;

namespace DecayEngine.NativeJS.Component.Script
{
    public class JsScriptComponent : IScript
    {
        private bool _initialized;
        private IGameObject _parentGameObject;
        private IScene _parentScene;

        public bool Destroyed { get; private set; }
        public string Name { get; set; }

        public IGameObject Parent => _parentGameObject;
        public ByReference<IGameObject> ParentByRef => () => ref _parentGameObject;

        IScene IParentable<IScene>.Parent => _parentScene;
        ByReference<IScene> IParentable<IScene>.ParentByRef => () => ref _parentScene;

        public Type ExportType => typeof(ScriptExport);
        public ScriptResource Resource { get; internal set; }

        public bool Persistent { get; set; }

        public bool Active
        {
            get
            {
                if (_parentScene == null && _parentGameObject == null)
                {
                    return false;
                }

                return _jsObject != null && _initialized;
            }
            set
            {
                if (!Active && value)
                {
                    Load();
                }
                else if (Active && !value)
                {
                    Unload();
                }
            }
        }

        public List<ScriptInjection> Injections { get; set; }

        private Module _jsModule;
        private JSObject _jsObject;

        public JsScriptComponent(Module jsModule)
        {
            Injections = new List<ScriptInjection>();

            _jsModule = jsModule;
        }

        public void SetParent(IGameObject parent)
        {
            _parentGameObject?.RemoveComponent(this);

            _parentScene?.RemoveComponent(this);
            _parentScene = null;

            parent?.AttachComponent(this);
            _parentGameObject = parent;
        }

        public IParentable<IGameObject> AsParentable<T>() where T : IGameObject
        {
            return this;
        }

        void IParentable<IScene>.SetParent(IScene parent)
        {
            _parentScene?.RemoveComponent(this);

            _parentGameObject?.RemoveComponent(this);
            _parentGameObject = null;

            parent?.AttachComponent(this);
            _parentScene = parent;
        }

        IParentable<IScene> IParentable<IScene>.AsParentable<T>()
        {
            return this;
        }

        public Task OnInit()
        {
            return _initialized ?
                Task.CompletedTask :
                Invoke("onInit", new GameObjectExport(Parent)).ContinueWith(task => _initialized = true);
        }

        public void ScriptUpdate(float deltaTime)
        {
            if (!_initialized) return;
            Invoke("onUpdate", deltaTime);
        }

        public void RenderUpdate(float deltaTime)
        {
            if (!_initialized) return;
            Invoke("onRender", deltaTime);
        }

        public object GetProperty(string propertyName)
        {
            if (!Active) return JSValue.Undefined;

            return GameEngine.ScriptEngine.EngineThread.ExecuteOnThread(() =>
                _jsObject.hasOwnProperty(new Arguments {propertyName}).As<bool>() ? _jsObject.GetProperty(propertyName) : JSValue.Undefined);
        }

        public void SetProperty(string propertyName, object value)
        {
            if (!Active) return;

            GameEngine.ScriptEngine.EngineThread.ExecuteOnThread(() =>
            {
                if (_jsObject.hasOwnProperty(new Arguments {propertyName}).As<bool>())
                {
                    _jsObject[propertyName] = (JSValue) GameEngine.ScriptEngine.MarshalTo(value);
                }
            });
        }

        private Task Invoke(string methodName, params object[] args)
        {
            try
            {
                return GameEngine.ScriptEngine.EngineThread.ExecuteOnThreadAsync(() => InvokeInternal(methodName, args));
            }
            catch (Exception e)
            {
                GameEngine.LogAppendLine(LogSeverity.Error, "NativeJS", e.Message);
                if (e.InnerException != null)
                {
                    GameEngine.LogAppendLine(LogSeverity.Error, "NativeJS", e.InnerException.Message);
                }

                return Task.CompletedTask;
            }
        }

        private Task<T> Invoke<T>(string methodName, params object[] args)
        {
            try
            {
                return GameEngine.ScriptEngine.EngineThread.ExecuteOnThreadAsync(() => InvokeInternal<T>(methodName, args));
            }
            catch (Exception e)
            {
                GameEngine.LogAppendLine(LogSeverity.Error, "NativeJS", e.Message);
                if (e.InnerException != null)
                {
                    GameEngine.LogAppendLine(LogSeverity.Error, "NativeJS", e.InnerException.Message);
                }

                return default;
            }
        }

        private void InvokeInternal(string methodName, params object[] args)
        {
            if (!_jsObject.Exists || !_jsObject.Defined)
            {
                throw new Exception("Javascript object representing script is undefined or null.");
            }

            JSValue property = _jsObject[methodName];
            if (property == null || !property.Exists || !property.Defined || !property.Is(JSValueType.Function))
            {
                throw new Exception($"Property \"{methodName}\" is undefined or not a function.");
            }

            Function method = property.As<Function>();
            Arguments jsArgs = new Arguments();
            foreach (object arg in args)
            {
                jsArgs.Add(arg);
            }

            if (method.RequireNewKeywordLevel == RequireNewKeywordLevel.WithNewOnly)
            {
                method.Construct(_jsObject, jsArgs);
            }
            else
            {
                method.Call(_jsObject, jsArgs);
            }
        }

        private T InvokeInternal<T>(string methodName, params object[] args)
        {
            if (!_jsObject.Exists || !_jsObject.Defined)
            {
                throw new Exception("Javascript object representing script is undefined or null.");
            }

            JSValue property = _jsObject[methodName];
            if (property == null || !property.Exists || !property.Defined || !property.Is(JSValueType.Function))
            {
                throw new Exception($"Property \"{methodName}\" is undefined or not a function.");
            }

            Function method = property.As<Function>();
            Arguments jsArgs = new Arguments();
            foreach (object arg in args)
            {
                jsArgs.Add(arg);
            }

            object result = method.RequireNewKeywordLevel == RequireNewKeywordLevel.WithNewOnly ?
                method.Construct(_jsObject, jsArgs).Value :
                method.Call(_jsObject, jsArgs).Value;

            if (result != null && result is T resCasted)
            {
                return resCasted;
            }
            throw new Exception($"Method \"{methodName}\" returned unexpected type {result?.GetType()}. Expected {typeof(T)}.");
        }

        private void Load()
        {
            GameEngine.ScriptEngine.EngineThread.ExecuteOnThread(() =>
            {
                try
                {
                    _jsObject = _jsModule.Exports.Default.As<Function>().Construct(new Arguments()).As<JSObject>();

                    _jsObject["self"] = (JSValue) GameEngine.ScriptEngine.MarshalTo(this);

                    if (_parentGameObject != null)
                    {
                        _jsObject["parent"] = (JSValue) GameEngine.ScriptEngine.MarshalTo(_parentGameObject);
                    }
                    else if (_parentScene != null)
                    {
                        _jsObject["parent"] = (JSValue) GameEngine.ScriptEngine.MarshalTo(_parentScene);
                    }

                    foreach (ScriptInjection injection in Injections)
                    {
                        if (injection.Id == "self") continue;

                        object injectionTarget = null;
                        QueryExpressionResolver resolver = new QueryExpressionResolver(injection.Expression);
                        if (_parentGameObject != null)
                        {
                            injectionTarget = resolver.Resolve(_parentGameObject);
                        }
                        else if (_parentScene != null)
                        {
                            injectionTarget = resolver.Resolve(_parentScene);
                        }

                        if (injectionTarget == null) continue;

                        _jsObject[injection.Id] = (JSValue) GameEngine.ScriptEngine.MarshalTo(injectionTarget);
                    }
                }
                catch (Exception e)
                {
                    if (e is JSException jsException)
                    {
                        if (jsException.CodeCoordinates != null && jsException.Code != null)
                        {
                            GameEngine.LogAppendLine(LogSeverity.Error, "NativeJS", jsException.Message,
                                jsException.CodeCoordinates.Line, "JavaScript", $"ScriptEnvironment({Name})");
                            GameEngine.LogAppendLine(LogSeverity.Error, "NativeJS", jsException.Code,
                                jsException.CodeCoordinates.Column, "JavaScript", "Column");
                        }
                        else
                        {
                            GameEngine.LogAppendLine(LogSeverity.Error, "NativeJS", e.Message,
                                0, "JavaScript", $"ScriptEnvironment({Name})");
                        }
                    }
                    else
                    {
                        GameEngine.LogAppendLine(LogSeverity.Error, "NativeJS", e.Message);
                        if (e.InnerException != null)
                        {
                            GameEngine.LogAppendLine(LogSeverity.Error, "NativeJS", e.InnerException.Message);
                        }
                    }
                }
            });
        }

        private void Unload()
        {
            GameEngine.ScriptEngine.EngineThread.ExecuteOnThread(() =>
            {
                _jsObject = null;
            });
        }

        public void Destroy()
        {
            Unload();

            SetParent(null);

            _jsModule = null;
            Resource = null;

            Destroyed = true;
        }
    }
}