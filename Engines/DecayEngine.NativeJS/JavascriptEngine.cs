using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using DecayEngine.ModuleSDK;
using DecayEngine.ModuleSDK.Capability;
using DecayEngine.ModuleSDK.Component;
using DecayEngine.ModuleSDK.Engine.Script;
using DecayEngine.ModuleSDK.Exports.Attributes;
using DecayEngine.ModuleSDK.Exports.BaseExports.GameObject;
using DecayEngine.ModuleSDK.Exports.BaseExports.Scene;
using DecayEngine.ModuleSDK.Logging;
using DecayEngine.ModuleSDK.Object;
using DecayEngine.ModuleSDK.Object.GameObject;
using DecayEngine.ModuleSDK.Object.Scene;
using DecayEngine.ModuleSDK.Threading;
using DecayEngine.NativeJS.Component.Script;
using NiL.JS.Core;
using NiL.JS.Extensions;
using Array = NiL.JS.BaseLibrary.Array;

namespace DecayEngine.NativeJS
{
    public class JavascriptEngine : IScriptEngine<JavascriptEngineOptions>
    {
        private GlobalContext _globalContext;

        public IEngineThread EngineThread { get; private set; }

        public List<IComponentFactory> ComponentFactories { get; }
        public ScriptExports ScriptExports { get; }

        public JavascriptEngine()
        {
            ComponentFactories = new List<IComponentFactory>
            {
                new JsScriptFactory()
            };

            ScriptExports = new ScriptExports();
        }

        public Task Init(JavascriptEngineOptions options)
        {
            EngineThread = new ManagedEngineThread("NativeJS", 128);
            Task initTask = EngineThread.ExecuteOnThreadAsync(InitEngine);

            EngineThread.Run();
            return initTask;
        }

        public Task Shutdown()
        {
            EngineThread.Stop();

            GameEngine.LogAppendLine(LogSeverity.Info, "NativeJS", "NativeJs terminated.");
            return Task.CompletedTask;
        }

        public Task InjectExports(ScriptExports exports)
        {
            return EngineThread.ExecuteOnThreadAsync(() =>
            {
                foreach (MethodInfo method in exports.Functions)
                {
                    InjectFunction(method);
                }

                foreach (KeyValuePair<string,object> pair in exports.GlobalVariables)
                {
                    _globalContext.DefineVariable(pair.Key).Assign(JSValue.Marshal(pair.Value));
                }

                foreach (Type type in exports.Types)
                {
                    InjectType(type);
                }
            });
        }

        private void InjectFunction(MethodInfo method)
        {
            if (!method.IsStatic) return;

            ScriptExportMethodAttribute attr = method.GetCustomAttribute<ScriptExportMethodAttribute>();
            if (attr == null || attr.DelegateType == null) return;

            Delegate del = Delegate.CreateDelegate(attr.DelegateType, method);

            if (attr.NameSpace == null)
            {
                _globalContext.DefineVariable(method.Name).Assign(JSValue.Marshal(del));
            }
            else
            {
                JSValue jsValue = UnwindNamespace(attr.NameSpace);
                jsValue?.DefineProperty(method.Name).Assign(JSValue.Marshal(del));
            }
        }

        private void InjectType(Type type)
        {
            ScriptExportBlockAttribute attr = type.GetCustomAttribute<ScriptExportBlockAttribute>();
            switch (attr)
            {
                case null:
                    break;
                case ScriptExportEnumAttribute enumAttribute:
                    InjectEnum(enumAttribute, type);
                    break;
                default:
                    if (attr.NameSpace == null)
                    {
                        JSValue existingValue = _globalContext.GetVariable(attr.Name);
                        if (existingValue == null || Equals(existingValue, JSValue.Undefined))
                        {
                            _globalContext.DefineConstructor(type, attr.Name);
                        }
                    }
                    else
                    {
                        JSValue jsValue = UnwindNamespace(attr.NameSpace);
                        JSObject constructor = _globalContext.GetConstructor(type);
                        jsValue?.DefineProperty(attr.Name).Assign(constructor);
                    }

                    break;
            }
        }

        private void InjectEnum(ScriptExportBlockAttribute attr, Type type)
        {
            if (!type.IsEnum) return;

            JSValue root = JSObject.create(new Arguments {new object()});
            if (attr.NameSpace == null)
            {
                JSValue existingValue = _globalContext.GetVariable(attr.Name);
                if (existingValue == null || Equals(existingValue, JSValue.Undefined))
                {
                    _globalContext.DefineVariable(attr.Name).Assign(root);
                }
            }
            else
            {
                JSValue jsValue = UnwindNamespace(attr.NameSpace);
                jsValue?.DefineProperty(attr.Name)?.Assign(root);
            }

            foreach (object enumValue in type.GetEnumValues())
            {
                root.DefineProperty(enumValue.ToString()).Assign(Convert.ChangeType(enumValue, Enum.GetUnderlyingType(type)));
            }
        }

        private JSValue UnwindNamespace(Type type)
        {
            Type nameSpace = type;
            List<string> nameSpaceNames = new List<string>();
            while (nameSpace != null)
            {
                ScriptExportNamespaceAttribute nameSpaceAttr = nameSpace.GetCustomAttribute<ScriptExportNamespaceAttribute>();
                if (nameSpaceAttr == null) break;

                nameSpaceNames.Add(nameSpaceAttr.Name);
                nameSpace = nameSpaceAttr.NameSpace;
            }

            nameSpaceNames.Reverse();
            JSValue jsValue = null;
            for (int i = 0; i < nameSpaceNames.Count; i++)
            {
                if (i == 0)
                {
                    JSValue previousJsValue = _globalContext.GetVariable(nameSpaceNames[0]);
                    if (previousJsValue != null)
                    {
                        jsValue = previousJsValue;
                    }
                    else
                    {
                        jsValue = _globalContext.DefineVariable(nameSpaceNames[0]);
                        jsValue.Assign(new object());
                    }
                }
                else
                {
                    JSValue previousJsValue = jsValue?.GetProperty(nameSpaceNames[i]);
                    if (previousJsValue != null && !previousJsValue.IsUndefined())
                    {
                        jsValue = previousJsValue;
                        continue;
                    }

                    JSValue innerJsValue = jsValue?.DefineProperty(nameSpaceNames[i]);
                    innerJsValue.Assign(new object());
                    jsValue = innerJsValue;
                }
            }

            return jsValue;
        }

        public object MarshalTo(object obj)
        {
            return EngineThread.ExecuteOnThread(() =>
            {
                return obj switch
                {
                    JSValue jsValue => jsValue,
                    IEnumerable enumerable => new Array(enumerable),
                    IComponent component => MarshalTo(component.GetComponentProxy()),
                    IGameObject gameObject => MarshalTo(new GameObjectExport(gameObject)),
                    IScene scene => MarshalTo(new SceneExport(scene)),
                    _ => (obj == null ? JSValue.Null : JSValue.Marshal(obj))
                };
            });
        }

        public T MarshalFrom<T>(object obj) where T : class, new()
        {
            return EngineThread.ExecuteOnThread(() =>
            {
                if (!(obj is JSValue jsValue)) return (T) obj;

                if (!typeof(IDictionary<string, object>).IsAssignableFrom(typeof(T))) return (T) jsValue.Value;

                if (jsValue.ValueType != JSValueType.Object) return (T) jsValue.Value;

                IDictionary<string, object> properties = (IDictionary<string, object>) new T();
                foreach (KeyValuePair<string,JSValue> pair in jsValue)
                {
                    properties[pair.Key] = pair.Value.Value;
                }

                return (T) (object) properties;
            });
        }

        private void InitEngine()
        {
            _globalContext = new GlobalContext();
            _globalContext.ActivateInCurrentThread();
            _globalContext.DeleteVariable("console");
            _globalContext.DeleteVariable("eval");

            _globalContext.Debugging = true;
            _globalContext.DebuggerCallback += DebuggerCallback;

            EngineThread.OnUpdate += Loop;

            GameEngine.LogAppendLine(LogSeverity.Info, "NativeJS", $"NativeJs loaded. Thread ID: {EngineThread.ThreadId}");
        }

        private void Loop(double deltaTime)
        {
            List<IScriptUpdateable> validUpdateables = GameEngine.Updateables
                .Where(updateable =>
                {
                    if (updateable == null || !updateable.Active)
                    {
                        return false;
                    }

                    return updateable switch
                    {
                        IParentable<IGameObject> parentable when parentable.Parent != null => true,
                        IParentable<IScene> sceneParentable when sceneParentable.Parent != null => true,
                        _ => false
                    };
                })
                .ToList();

            Task[] updateTasks = new Task[validUpdateables.Count];
            for (int i = 0; i < validUpdateables.Count; i++)
            {
                IScriptUpdateable renderable = validUpdateables[i];
                Task renderTask = EngineThread.ExecuteOnThreadAsync(() => renderable.ScriptUpdate((float) deltaTime));
                updateTasks[i] = renderTask;
            }
            Task.WaitAll(updateTasks.ToArray());
        }

        private static void DebuggerCallback(Context sender, DebuggerCallbackEventArgs e)
        {
//            Debugger.Break();
        }

//        private void SetStrict(bool value)
//        {
//            typeof(GlobalContext)
//                .GetField("_strict", BindingFlags.Instance | BindingFlags.NonPublic)?.SetValue(_globalContext, true);
//        }
    }
}