using DecayEngine.ModuleSDK.Exports.Attributes;

namespace DecayEngine.ModuleSDK.Exports.BaseExports.TextSprite
{
    [ScriptExportEnum("TextAlignmentHorizontal", "Represents how text should be drawn on the horizontal axis.")]
    public enum TextAlignmentHorizontalExport
    {
        [ScriptExportField("The text should be drawn starting from the leftmost point of its bounding box.\n" +
        "The text will overflow to the right side of its bounding box from this point until it spans the entirety of the bounding box's width or\n" +
        "the line ends.")]
        HLeft = 0,
        [ScriptExportField("The text should be drawn at the center of its bounding box.\n" +
        "The text will overflow to both sides of its bounding box from this point until it spans the entirety of the bounding box's width or all the\n" +
        "characters of the line have been drawn.")]
        HCenter = 1,
        [ScriptExportField("The text should be drawn starting from the rightmost point of its bounding box.\n" +
        "The text will overflow to the left side of its bounding box from this point until it spans the entirety of the bounding box's width or\n" +
        "the line ends.")]
        HRight = 2
    }
}