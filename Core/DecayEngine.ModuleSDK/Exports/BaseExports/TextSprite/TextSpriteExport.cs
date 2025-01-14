using DecayEngine.DecPakLib;
using DecayEngine.ModuleSDK.Capability;
using DecayEngine.ModuleSDK.Component.Sprite;
using DecayEngine.ModuleSDK.Engine.Render;
using DecayEngine.ModuleSDK.Exports.Attributes;
using DecayEngine.ModuleSDK.Exports.BaseExports.GameObject;
using DecayEngine.ModuleSDK.Exports.BaseExports.ShaderProgram;
using DecayEngine.ModuleSDK.Exports.BaseExports.Transform;
using DecayEngine.ModuleSDK.Exports.Capabilities;

namespace DecayEngine.ModuleSDK.Exports.BaseExports.TextSprite
{
    [ScriptExportClass("TextSprite", "Represents a TextSprite Component.")]
    public class TextSpriteExport : ExportableManagedObject<ITextSprite>, IComponentExport, IDrawableExport
    {
        public bool Active
        {
            get => Reference.Active;
            set => Reference.Active = value;
        }

        public bool ActiveInGraph => Reference.ActiveInGraph();

        public string Name
        {
            get => Reference.Name;
            set => Reference.Name = value;
        }

        public string Id => Reference.Resource.Id;

        public object Parent => Reference.Parent != null ? new GameObjectExport(Reference.Parent) : null;

        public TransformExport Transform => new TransformExport(Reference.TransformByRef);
        public TransformExport WorldSpaceTransform => new TransformExport(((ITransformable) Reference).WorldSpaceTransform);

        [ScriptExportProperty("The `ShaderProgram` used to render the `TextSprite`.")]
        public ShaderProgramExport ShaderProgram
        {
            get => new ShaderProgramExport(Reference.ShaderProgramByRef);
            set => Reference.ShaderProgram = value.Value;
        }

        public bool ShouldDraw
        {
            get => Reference.ShouldDraw;
            set => Reference.ShouldDraw = value;
        }

        [ScriptExportProperty("The text the `TextSprite` will render.")]
        public string Text
        {
            get => Reference.Text;
            set => Reference.Text = value;
        }

        [ScriptExportProperty("The size of the font the `TextSprite` will render.\n" +
        "WARNING: This is NOT the same as the scale of the transform of the `TextSprite`, this property is applied before any spatial scaling is applied.\n" +
        "It is recommended to scale text using this property instead of adjusting the scale of the transform as this will do a better job at preventing\n" +
        "aliasing of the glyphs.")]
        public float FontSize
        {
            get => Reference.FontSize;
            set => Reference.FontSize = value;
        }

        [ScriptExportProperty("The separation the `TextSprite` will keep between characters.\n" +
        "A value of 1 means the default character separation of the used font before kerning is applied.\n" +
        "The value of this property represents a scaling factor applied to the default value.")]
        public float CharacterSeparation
        {
            get => Reference.CharacterSeparation;
            set => Reference.CharacterSeparation = value;
        }

        [ScriptExportProperty("The width the `TextSprite` will use to represent white spaces.\n" +
        "A value of 1 means the default white space character width of the used font before kerning is applied.\n" +
        "The value of this property represents a scaling factor applied to the default value.")]
        public float WhiteSpaceSeparation
        {
            get => Reference.WhiteSpaceSeparation;
            set => Reference.WhiteSpaceSeparation = value;
        }

        [ScriptExportProperty("The horizontal alignment of the `TextSprite`.", typeof(TextAlignmentHorizontalExport))]
        public int AlignmentHorizontal
        {
            get => (int) Reference.AlignmentHorizontal;
            set => Reference.AlignmentHorizontal = (TextAlignmentHorizontal) value;
        }

        [ScriptExportProperty("The vertical alignment of the `TextSprite`.", typeof(TextAlignmentVerticalExport))]
        public int AlignmentVertical
        {
            get => (int) Reference.AlignmentVertical;
            set => Reference.AlignmentVertical = (TextAlignmentVertical) value;
        }

        public override int Type => (int) ManagedExportType.TextSprite;

        [ExportCastConstructor]
        internal TextSpriteExport(ByReference<ITextSprite> referencePointer) : base(referencePointer)
        {
        }

        [ExportCastConstructor]
        internal TextSpriteExport(ITextSprite value) : base(value)
        {
        }
    }
}