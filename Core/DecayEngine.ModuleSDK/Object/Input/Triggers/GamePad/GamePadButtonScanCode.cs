using DecayEngine.ModuleSDK.Exports.Attributes;

namespace DecayEngine.ModuleSDK.Object.Input.Triggers.GamePad
{
    [ScriptExportEnum("GamePadButtonScanCode", "Represents the buttons of a GamePad.")]
    public enum GamePadButtonScanCode
    {
        [ScriptExportField("Unsupported button.")]
        Unsupported = 0,
        [ScriptExportField("A button.")]
        A = 1,
        [ScriptExportField("B button.")]
        B = 2,
        [ScriptExportField("X button.")]
        X = 3,
        [ScriptExportField("Y button.")]
        Y = 4,
        [ScriptExportField("Back button.")]
        Back = 5,
        [ScriptExportField("Guide/Select button.")]
        Guide = 6,
        [ScriptExportField("Start button.")]
        Start = 7,
        [ScriptExportField("L3 (left stick click) button.")]
        L3 = 8,
        [ScriptExportField("R3 (right stick click) button.")]
        R3 = 9,
        [ScriptExportField("L1 (left shoulder) button.")]
        L1 = 10,
        [ScriptExportField("R1 (right shoulder) button.")]
        R1 = 11,
        [ScriptExportField("Up (dpad) button.")]
        Up = 12,
        [ScriptExportField("Down (dpad) button.")]
        Down = 13,
        [ScriptExportField("Left (dpad) button.")]
        Left = 14,
        [ScriptExportField("Right (dpad) button.")]
        Right = 15
    }
}