using System.Collections.Generic;
using System.Linq;
using DecayEngine.ModuleSDK.Exports.Attributes;

namespace DecayEngine.ModuleSDK.Exports.BaseExports.Input
{
    [ScriptExportNamespace("InputController", "Provides functionality to handle input.")]
    public static class InputControllerExport
    {
        [ScriptExportProperty("The currently active Action Map.", typeof(IEnumerable<InputActionExport>),
            typeof(InputControllerExport))]
        public static object ActionMap =>
            GameEngine.ScriptEngine.MarshalTo(GameEngine.ActionMap.Select(a => new InputActionExport(a.Value) {Name = a.Key}).ToList());

        private delegate InputActionExport CreateInputActionDelegate(string name);
        [ScriptExportMethod("Creates a new `InputAction`.", typeof(InputControllerExport), typeof(CreateInputActionDelegate))]
        [return: ScriptExportReturn("The newly created `InputAction`.")]
        public static InputActionExport CreateInputAction(
            [ScriptExportParameter("The name of the `InputAction` to create.")] string name
        )
        {
            return new InputActionExport(GameEngine.CreateInputAction(name));
        }

        private delegate void RemoveInputActionDelegate(string name);
        [ScriptExportMethod("Removes an `InputAction`.", typeof(InputControllerExport), typeof(RemoveInputActionDelegate))]
        public static void RemoveInputAction(
            [ScriptExportParameter("The name of the `InputAction` to remove.")] string name
        )
        {
            GameEngine.RemoveInputAction(name);
        }
    }
}