using DecayEngine.DecPakLib;
using DecayEngine.ModuleSDK.Exports.Attributes;
using DecayEngine.ModuleSDK.Object.Input;
using DecayEngine.ModuleSDK.Object.Input.Triggers.GamePad;

namespace DecayEngine.ModuleSDK.Exports.BaseExports.Input.Triggers.GamePad
{
    [ScriptExportClass("GamePadAxisActionTrigger", "Represents a GamePad Axis Input Action Trigger.")]
    public class GamePadAxisActionTriggerExport : InputActionTriggerAnalogExport
    {
        public override string SubType => "GamePadAxisActionTrigger";

        [ScriptExportProperty("The scan code of the button the action trigger will listen for.", typeof(GamePadAxisScanCode))]
        public int ScanCode => (int) ((IGamePadAxisActionTrigger) Reference).ScanCode;

        [ScriptExportProperty("The index of the gamepad the action trigger will listen for.")]
        public int GamePadIndex => ((IGamePadAxisActionTrigger) Reference).GamePadIndex;

        [ExportCastConstructor]
        public GamePadAxisActionTriggerExport(ByReference<IInputActionTrigger> referencePointer) : base(referencePointer)
        {
        }

        [ExportCastConstructor]
        public GamePadAxisActionTriggerExport(IInputActionTrigger value) : base(value)
        {
        }
    }
}