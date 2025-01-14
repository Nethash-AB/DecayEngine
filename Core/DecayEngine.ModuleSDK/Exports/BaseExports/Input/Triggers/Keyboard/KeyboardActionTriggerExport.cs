using DecayEngine.DecPakLib;
using DecayEngine.ModuleSDK.Engine.Input;
using DecayEngine.ModuleSDK.Exports.Attributes;
using DecayEngine.ModuleSDK.Object.Input;
using DecayEngine.ModuleSDK.Object.Input.Triggers.Keyboard;

namespace DecayEngine.ModuleSDK.Exports.BaseExports.Input.Triggers.Keyboard
{
    [ScriptExportClass("KeyboardActionTrigger", "Represents a Keyboard Input Action Trigger.")]
    public class KeyboardActionTriggerExport : InputActionTriggerDigitalExport
    {
        public override string SubType => "KeyboardActionTrigger";

        [ScriptExportProperty("The scan code of the key the action will listen for.", typeof(KeyboardScanCode))]
        public int ScanCode => (int) ((IKeyboardActionTrigger) Reference).ScanCode;

        [ExportCastConstructor]
        public KeyboardActionTriggerExport(ByReference<IInputActionTrigger> referencePointer) : base(referencePointer)
        {
        }

        [ExportCastConstructor]
        public KeyboardActionTriggerExport(IInputActionTrigger value) : base(value)
        {
        }
    }
}