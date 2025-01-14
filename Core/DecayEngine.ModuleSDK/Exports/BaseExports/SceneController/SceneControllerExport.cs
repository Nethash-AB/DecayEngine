using System;
using System.Collections.Generic;
using System.Linq;
using DecayEngine.DecPakLib.Resource.RootElement.Scene;
using DecayEngine.ModuleSDK.Exports.Attributes;
using DecayEngine.ModuleSDK.Exports.BaseExports.Scene;
using DecayEngine.ModuleSDK.Logging;
using DecayEngine.ModuleSDK.Object;
using DecayEngine.ModuleSDK.Object.Scene;

namespace DecayEngine.ModuleSDK.Exports.BaseExports.SceneController
{
    [ScriptExportNamespace("SceneController", "Provides functionality to handle scenes.")]
    public static class SceneControllerExport
    {
        [ScriptExportProperty("The currently loaded `Scene`.", (Type) null, typeof(SceneControllerExport))]
        public static SceneExport ActiveScene => new SceneExport(GameEngine.ActiveScene);

        [ScriptExportProperty("The list of the resource ids of all the available scenes.", typeof(IEnumerable<string>),
            typeof(SceneControllerExport))]
        public static object AvailableScenes =>
            GameEngine.ScriptEngine.MarshalTo(GameEngine.ResourceBundles.SelectMany(bundle => bundle.Resources).OfType<SceneResource>());

        private delegate void ChangeSceneDelegate(string id);
        [ScriptExportMethod("Unloads the current `Scene` and loads the requested `Scene`.", typeof(SceneControllerExport),
            typeof(ChangeSceneDelegate))]
        public static void ChangeScene(
            [ScriptExportParameter("The id of the resource of the `IScene` to load.")] string id
        )
        {
            IScene scene = GameEngine.Scenes.FirstOrDefault(scn => scn.Name == id);
            if (scene == null)
            {
                GameEngine.LogAppendLine(LogSeverity.Error, "SceneControllerExtension", $"Error: Scene ({id}) does not exist.");
                return;
            }
            GameEngine.ChangeScene(scene);
        }
    }
}