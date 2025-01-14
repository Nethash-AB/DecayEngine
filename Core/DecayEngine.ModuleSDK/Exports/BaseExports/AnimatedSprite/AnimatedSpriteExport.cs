using DecayEngine.DecPakLib;
using DecayEngine.ModuleSDK.Capability;
using DecayEngine.ModuleSDK.Component.Material;
using DecayEngine.ModuleSDK.Component.ShaderProgram;
using DecayEngine.ModuleSDK.Component.Sprite;
using DecayEngine.ModuleSDK.Exports.Attributes;
using DecayEngine.ModuleSDK.Exports.BaseExports.AnimatedMaterial;
using DecayEngine.ModuleSDK.Exports.BaseExports.GameObject;
using DecayEngine.ModuleSDK.Exports.BaseExports.ShaderProgram;
using DecayEngine.ModuleSDK.Exports.BaseExports.Transform;
using DecayEngine.ModuleSDK.Exports.Capabilities;
using DecayEngine.ModuleSDK.Math.Exports;

namespace DecayEngine.ModuleSDK.Exports.BaseExports.AnimatedSprite
{
    [ScriptExportClass("AnimatedSprite", "Represents an Animated Sprite Component.")]
    public class AnimatedSpriteExport : ExportableManagedObject<IAnimatedSprite>, IComponentExport, IDrawableExport
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

        public string Id => null;

        public object Parent => Reference.Parent != null ? new GameObjectExport(Reference.Parent) : null;

        [ScriptExportProperty("The transform of the `AnimatedSprite`.")]
        public TransformExport Transform => new TransformExport(Reference.TransformByRef);
        public TransformExport WorldSpaceTransform => new TransformExport(((ITransformable) Reference).WorldSpaceTransform);

        [ScriptExportProperty("The `AnimatedMaterial` used to render the `AnimatedSprite`.")]
        public AnimatedMaterialExport Material
        {
            get => new AnimatedMaterialExport((IAnimatedMaterial) Reference.Material);
            set => Reference.Material = value.Value;
        }

        [ScriptExportProperty("The `ShaderProgram` used to render the `AnimatedSprite`.")]
        public ShaderProgramExport ShaderProgram
        {
            get => new ShaderProgramExport(Reference.ShaderProgramByRef);
            set => Reference.ShaderProgram = value.Value;
        }

        [ScriptExportProperty("The current frame of the `AnimatedSprite`." +
        "\nThis value corresponds to one of the frame indices of the active `TextureBundle`." +
        "\nWARNING: Changing the frame causes a GPU buffering operation, this property should only be updated from `onRender`.")]
        public int Frame
        {
            get => Reference.Frame;
            set => Reference.Frame = value;
        }

        [ScriptExportProperty("The total amount of frames present on the active `AnimatedMaterial`." +
        "\nWARNING: This property is the total AMOUNT of frames, NOT the last index.")]
        public int FrameCount => ((IAnimatedMaterial) Reference.Material)?.AnimationFrames.Count ?? 0;

        public bool ShouldDraw
        {
            get => Reference.ShouldDraw;
            set => Reference.ShouldDraw = value;
        }

        public override int Type => (int) ManagedExportType.Sprite2D;

        [ScriptExportConstructor]
        public AnimatedSpriteExport(
            [ScriptExportParameter("The name of the resource of the `TextureBundle` to use for the `AnimatedSprite`.")] string textureName,
            [ScriptExportParameter("The name of the resource of the `ShaderProgram` to use for the `AnimatedSprite`.")] string shaderProgramName
        ) : this(textureName, shaderProgramName, Vector3Export.Zero, QuaternionExport.Identity, new Vector3Export(1f, 1f, 0f))
        {
        }

        [ScriptExportConstructor]
        public AnimatedSpriteExport(
            [ScriptExportParameter("The name of the resource of the `TextureBundle` to use for the `AnimatedSprite`.")] string textureName,
            [ScriptExportParameter("The name of the resource of the `ShaderProgram` to use for the `AnimatedSprite`.")] string shaderProgramName,
            [ScriptExportParameter("The target position of the `AnimatedSprite`.")] Vector3Export position,
            [ScriptExportParameter("The target rotation of the `AnimatedSprite`.")] QuaternionExport rotation
        ) : this(textureName, shaderProgramName, position, rotation, new Vector3Export(1f, 1f, 0f))
        {
        }

        [ScriptExportConstructor]
        public AnimatedSpriteExport(
            [ScriptExportParameter("The name of the resource of the `TextureBundle` to use for the `AnimatedSprite`.")] string textureName,
            [ScriptExportParameter("The name of the resource of the `ShaderProgram` to use for the `AnimatedSprite`.")] string shaderProgramName,
            [ScriptExportParameter("The target position of the `AnimatedSprite`.")] Vector3Export position
        ) : this(textureName, shaderProgramName, position, QuaternionExport.Identity, new Vector3Export(1f, 1f, 0f))
        {
        }

        [ScriptExportConstructor]
        public AnimatedSpriteExport(
            [ScriptExportParameter("The name of the resource of the `AnimatedMaterial` to use for the `AnimatedSprite`.")] string materialName,
            [ScriptExportParameter("The name of the resource of the `ShaderProgram` to use for the `AnimatedSprite`.")] string shaderProgramName,
            [ScriptExportParameter("The target position of the `AnimatedSprite`.")] Vector3Export position,
            [ScriptExportParameter("The target rotation of the `AnimatedSprite`.")] QuaternionExport rotation,
            [ScriptExportParameter("The target scale of the `AnimatedSprite`.")] Vector3Export scale
        )
        {
            Reference = GameEngine.RenderEngine.EngineThread.ExecuteOnThread(() =>
            {
                IAnimatedSprite component = GameEngine.CreateComponent<IAnimatedSprite>();
                component.Transform.Position = position;
                component.Transform.Rotation = rotation;
                component.Transform.Scale = scale;
                component.Material = GameEngine.CreateComponent<IAnimatedMaterial>(materialName);
                component.ShaderProgram = GameEngine.CreateComponent<IShaderProgram>(shaderProgramName);
                component.Material.Active = true;
                component.ShaderProgram.Active = true;

                return component;
            });
        }

        [ExportCastConstructor]
        internal AnimatedSpriteExport(ByReference<IAnimatedSprite> referencePointer) : base(referencePointer)
        {
        }

        [ExportCastConstructor]
        internal AnimatedSpriteExport(IAnimatedSprite value) : base(value)
        {
        }
    }
}