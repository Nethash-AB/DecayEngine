using System;
using System.IO;
using DecayEngine.DecPakLib.Resource.RootElement.Script;
using DecayEngine.ModuleSDK;
using DecayEngine.ModuleSDK.Component;
using DecayEngine.ModuleSDK.Logging;
using NiL.JS;
using NiL.JS.BaseLibrary;
using NiL.JS.Extensions;

namespace DecayEngine.NativeJS.Component.Script
{
    public class JsScriptFactory : IComponentFactory<JsScriptComponent, ScriptResource>
    {
        public JsScriptComponent CreateComponent(ScriptResource resource)
        {
            return GameEngine.ScriptEngine.EngineThread.ExecuteOnThread(() =>
            {
                string scriptSrc;
                using (TextReader reader = new StreamReader(resource.Source.GetData(), true))
                {
                    scriptSrc = reader.ReadToEnd();
                }

                Module module = new Module($"{resource.Source.Package.RelativePath}/{resource.Source.SourcePath}", scriptSrc,
                    (level, coords, message) =>
                    {
                        switch (level)
                        {
                            case MessageLevel.Error:
                            case MessageLevel.CriticalWarning:
                                GameEngine.LogAppendLine(LogSeverity.Error, "NativeJS", message,
                                    coords.Line, resource.Id, "ScriptEnvironment");
                                break;
                            case MessageLevel.Warning:
                                GameEngine.LogAppendLine(LogSeverity.Warning, "NativeJS", message,
                                    coords.Line, resource.Id, "ScriptEnvironment");
                                break;
                            default:
                                GameEngine.LogAppendLine(LogSeverity.Debug, "NativeJS", message,
                                    coords.Line, resource.Id, "ScriptEnvironment");
                                break;
                        }
                    });
                try
                {
                    module.Run();
                }
                catch (Exception e)
                {
                    throw new Exception($"Error loading script {resource.Source.SourcePath} from {resource.Source.Package.RelativePath}: {e.Message}");
                }

                Function prototype = module.Exports.Default.As<Function>();
                return new JsScriptComponent(module)
                {
                    Name = prototype.name,
                    Resource = resource
                };
            });
        }
    }
}