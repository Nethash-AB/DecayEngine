using DecayEngine.ModuleSDK.Exports.Attributes;

namespace DecayEngine.ModuleSDK.Object.Input.Triggers.GamePad
{
    [ScriptExportEnum("GamePadAxisScanCode", "Represents the axes of a GamePad.")]
    public enum GamePadAxisScanCode
    {
        [ScriptExportField("Unsupported axis.")]
        Unsupported = 0,
        [ScriptExportField("Left stick X axis.")]
        LeftX = 1,
        [ScriptExportField("Left stick Y axis.")]
        LeftY = 2,
        [ScriptExportField("Right stick X axis.")]
        RightX = 3,
        [ScriptExportField("Right stick Y axis.")]
        RightY = 4,
        [ScriptExportField("L2 (left trigger) axis.")]
        L2 = 5,
        [ScriptExportField("R2 (right trigger) axis.")]
        R2 = 6
    }
}