using DecayEngine.ModuleSDK.Exports.Attributes;

namespace DecayEngine.ModuleSDK.Exports.BaseExports.TextSprite
{
    [ScriptExportEnum("TextAlignmentHorizontal", "Represents how text should be drawn on the vertical axis.")]
    public enum TextAlignmentVerticalExport
    {
        [ScriptExportField("The text should be drawn starting from the topmost point of its bounding box.\n" +
        "The text will overflow to the bottom side of its bounding box from this point until it spans the entirety of the bounding box's height or\n" +
        "all lines have been drawn.")]
        VTop = 3,
        [ScriptExportField("The text should be drawn at the center of its bounding box.\n" +
        "The text will overflow to both the bottom and the top of its bounding box from this point until it spans the entirety of the bounding box's height\n" +
        "or all lines have been drawn.")]
        VCenter = 4,
        [ScriptExportField("The text should be drawn starting from the bottom most point of its bounding box.\n" +
        "The text will overflow to the top side of its bounding box from this point until it spans the entirety of the bounding box's height or\n" +
        "all lines have been drawn.")]
        VBottom = 5
    }
}