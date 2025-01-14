using DecayEngine.DecPakLib;
using DecayEngine.ModuleSDK.Exports.Attributes;
using DecayEngine.ModuleSDK.Object.Input;
using DecayEngine.ModuleSDK.Object.Input.Triggers.GamePad;

namespace DecayEngine.ModuleSDK.Exports.BaseExports.Input.Triggers.GamePad
{
    [ScriptExportClass("GamePadButtonActionTrigger", "Represents a GamePad Button Input Action Trigger.")]
    public class GamePadButtonActionTriggerExport : InputActionTriggerDigitalExport
    {
        public override string SubType => "GamePadButtonActionTrigger";

        [ScriptExportProperty("The scan code of the button the action trigger will listen for.", typeof(GamePadButtonScanCode))]
        public int ScanCode => (int) ((IGamePadButtonActionTrigger) Reference).ScanCode;

        [ScriptExportProperty("The index of the gamepad the action trigger will listen for.")]
        public int GamePadIndex => ((IGamePadButtonActionTrigger) Reference).GamePadIndex;

        [ExportCastConstructor]
        public GamePadButtonActionTriggerExport(ByReference<IInputActionTrigger> referencePointer) : base(referencePointer)
        {
        }

        [ExportCastConstructor]
        public GamePadButtonActionTriggerExport(IInputActionTrigger value) : base(value)
        {
        }
    }
}