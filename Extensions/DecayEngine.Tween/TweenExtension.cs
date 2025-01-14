using System.Collections.Generic;
using System.Threading.Tasks;
using DecayEngine.ModuleSDK;
using DecayEngine.ModuleSDK.Component;
using DecayEngine.ModuleSDK.Engine.Extension;
using DecayEngine.ModuleSDK.Engine.Script;
using DecayEngine.ModuleSDK.Logging;
using DecayEngine.Tween.Exports.Tween;

namespace DecayEngine.Tween
{
    public class TweenExtension : IEngineExtension<TweenExtensionOptions>
    {
        public List<IComponentFactory> ComponentFactories { get; }
        public ScriptExports ScriptExports { get; }

        public TweenExtension()
        {
            ComponentFactories = new List<IComponentFactory>();

            ScriptExports = new ScriptExports();
            ScriptExports.Types.Add(typeof(TweenEaseType));
            ScriptExports.Types.Add(typeof(TweenExport));
        }

        public Task Init(TweenExtensionOptions options)
        {
            GameEngine.LogAppendLine(LogSeverity.Info, "TweenExtension", "Tween loaded.");

            return Task.CompletedTask;
        }

        public Task Shutdown()
        {
            GameEngine.LogAppendLine(LogSeverity.Info, "TweenExtension", "Tween terminated.");
            return Task.CompletedTask;
        }
    }
}