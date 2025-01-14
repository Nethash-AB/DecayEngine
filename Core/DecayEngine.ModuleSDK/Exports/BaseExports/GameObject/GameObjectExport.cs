using System.Collections.Generic;
using System.Linq;
using DecayEngine.DecPakLib;
using DecayEngine.ModuleSDK.Capability;
using DecayEngine.ModuleSDK.Exports.Attributes;
using DecayEngine.ModuleSDK.Exports.BaseExports.Scene;
using DecayEngine.ModuleSDK.Exports.BaseExports.Transform;
using DecayEngine.ModuleSDK.Exports.Capabilities;
using DecayEngine.ModuleSDK.Object;
using DecayEngine.ModuleSDK.Object.GameObject;
using DecayEngine.ModuleSDK.Object.Scene;

namespace DecayEngine.ModuleSDK.Exports.BaseExports.GameObject
{
    [ScriptExportClass("GameObject", "Represents a GameObject.")]
    public class GameObjectExport
        : ExportableManagedObject<IGameObject>, IActivableExport, INameableExport, ITransformableExport, IComponentableExport, IParentableExport
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

        public override int Type => (int) ManagedExportType.GameObject;

        [ScriptExportProperty("The transform of the `GameObject`.")]
        public TransformExport Transform => new TransformExport(Reference.TransformByRef);
        public TransformExport WorldSpaceTransform => new TransformExport(Reference.WorldSpaceTransform);

        public object Components => GameEngine.ScriptEngine.MarshalTo(Reference.Components.Select(component => component.GetComponentProxy()));

        public void AttachComponent(IComponentExport component)
        {
            Reference.AttachComponent(component.GetRealComponent());
        }

        public void AttachComponents(IEnumerable<IComponentExport> components)
        {
            Reference.AttachComponents(components.Select(componentExport => componentExport.GetRealComponent()));
        }

        public void RemoveComponent(IComponentExport component)
        {
            Reference.RemoveComponent(component.GetRealComponent());
        }

        public void RemoveComponents(IEnumerable<IComponentExport> components)
        {
            Reference.RemoveComponents(components.Select(componentExport => componentExport.GetRealComponent()));
        }

        [ExportCastConstructor]
        public GameObjectExport(ByReference<IGameObject> referencePointer) : base(referencePointer)
        {
        }

        [ExportCastConstructor]
        public GameObjectExport(IGameObject value) : base(value)
        {
        }
    }
}