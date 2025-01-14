using System.Collections.Generic;
using System.Linq;
using DecayEngine.DecPakLib;
using DecayEngine.ModuleSDK.Capability;
using DecayEngine.ModuleSDK.Component.Camera;
using DecayEngine.ModuleSDK.Exports.Attributes;
using DecayEngine.ModuleSDK.Exports.BaseExports.GameObject;
using DecayEngine.ModuleSDK.Exports.BaseExports.Scene;
using DecayEngine.ModuleSDK.Exports.BaseExports.Transform;
using DecayEngine.ModuleSDK.Exports.Capabilities;
using DecayEngine.ModuleSDK.Math.Exports;
using DecayEngine.ModuleSDK.Object;
using DecayEngine.ModuleSDK.Object.GameObject;
using DecayEngine.ModuleSDK.Object.Scene;

namespace DecayEngine.ModuleSDK.Exports.BaseExports.Camera
{
    [ScriptExportClass("Camera", "Represents a Camera Component.")]
    public class CameraExport : ExportableManagedObject<ICamera>, IComponentExport, ITransformableExport
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

        public object Parent
        {
            get
            {
                IScene parentScene = Reference.AsParentable<IScene>().Parent;
                if (parentScene != null) return new SceneExport(parentScene);

                IGameObject parentGo = Reference.AsParentable<IGameObject>().Parent;
                return parentGo != null ? new GameObjectExport(parentGo) : null;
            }
        }

        [ScriptExportProperty("The transform of the `Camera`.")]
        public TransformExport Transform => new TransformExport(() => ref Reference.TransformByRef.Invoke());
        public TransformExport WorldSpaceTransform => new TransformExport(Reference.WorldSpaceTransform);

        [ScriptExportProperty("The near value determines the minimal distance between the `Camera` and a `Sprite2D`." +
                              "\nIf a `Sprite2D` is closer to the camera than this distance, the `Sprite2D` is clipped and does not appear.")]
        public float ZNear
        {
            get => Reference.ZNear;
            set => Reference.ZNear = value;
        }

        [ScriptExportProperty("The far value determines the maximal distance between the `Camera` and a `Sprite2D`." +
                              "\nIf a `Sprite2D` is farther from the `Camera` than this distance, the `Sprite2D` is clipped and does not appear.")]
        public float ZFar
        {
            get => Reference.ZFar;
            set => Reference.ZFar = value;
        }

        [ScriptExportProperty("The list of drawables the camera will render.", typeOverride: typeof(IEnumerable<IDrawableExport>))]
        public object Drawables => GameEngine.ScriptEngine.MarshalTo(Reference.Drawables.Select(drawable => drawable.GetDrawableProxy()).ToList());

        public override int Type => (int) ManagedExportType.Camera;

        [ScriptExportMethod("Adds a drawable to the list of drawables the camera will render.")]
        public void AddDrawable(
            [ScriptExportParameter("The drawable to add.")] IDrawableExport drawable
        )
        {
            Reference.AddDrawable(drawable.GetRealDrawable());
        }

        [ScriptExportMethod("Removes a drawable to the list of drawables the camera will render.")]
        public void RemoveDrawable(
            [ScriptExportParameter("The drawable to remove.")] IDrawableExport drawable
        )
        {
            Reference.RemoveDrawable(drawable.GetRealDrawable());
        }

        protected CameraExport(ICamera component, Vector3Export position, QuaternionExport rotation) : base(component)
        {
            Transform.Position = position;
            Transform.Rotation = rotation;
            Transform.Scale = Vector3Export.One;
        }

        [ExportCastConstructor]
        internal CameraExport(ByReference<ICamera> referencePointer) : base(referencePointer)
        {
        }

        [ExportCastConstructor]
        internal CameraExport(ICamera value) : base(value)
        {
        }
    }
}