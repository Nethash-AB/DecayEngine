using System.Collections.Generic;
using System.Linq;
using DecayEngine.DecPakLib;
using DecayEngine.ModuleSDK.Capability;
using DecayEngine.ModuleSDK.Component;
using DecayEngine.ModuleSDK.Exports.Attributes;
using DecayEngine.ModuleSDK.Exports.Capabilities;
using DecayEngine.ModuleSDK.Object;
using DecayEngine.ModuleSDK.Object.Scene;

namespace DecayEngine.ModuleSDK.Exports.BaseExports.Scene
{
    [ScriptExportClass("Scene", "Represents a Scene.")]
    public class SceneExport : ExportableManagedObject<IScene>, IActivableExport, INameableExport, IComponentableExport
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
            set {}
        }

        public string Id => Reference.Resource.Id;

        public override int Type => (int) ManagedExportType.Scene;

        public object Components => GameEngine.ScriptEngine.MarshalTo(Reference.Components.Select(component => component.GetComponentProxy()));

        public void AttachComponent(IComponentExport component)
        {
            if (!(component.GetRealComponent() is ISceneAttachableComponent sceneAttachableComponent)) return;
            Reference.AttachComponent(sceneAttachableComponent);
        }

        public void AttachComponents(IEnumerable<IComponentExport> components)
        {
            List<ISceneAttachableComponent> sceneAttachableComponents = new List<ISceneAttachableComponent>();
            foreach (IComponentExport component in components)
            {
                if (!(component.GetRealComponent() is ISceneAttachableComponent sceneAttachableComponent)) continue;
                sceneAttachableComponents.Add(sceneAttachableComponent);
            }
            Reference.AttachComponents(sceneAttachableComponents);
        }

        public void RemoveComponent(IComponentExport component)
        {
            if (!(component.GetRealComponent() is ISceneAttachableComponent sceneAttachableComponent)) return;
            Reference.RemoveComponent(sceneAttachableComponent);
        }

        public void RemoveComponents(IEnumerable<IComponentExport> components)
        {
            List<ISceneAttachableComponent> sceneAttachableComponents = new List<ISceneAttachableComponent>();
            foreach (IComponentExport component in components)
            {
                if (!(component.GetRealComponent() is ISceneAttachableComponent sceneAttachableComponent)) continue;
                sceneAttachableComponents.Add(sceneAttachableComponent);
            }
            Reference.RemoveComponents(sceneAttachableComponents);
        }

        [ExportCastConstructor]
        public SceneExport(ByReference<IScene> referencePointer) : base(referencePointer)
        {
        }

        [ExportCastConstructor]
        public SceneExport(IScene value) : base(value)
        {
        }
    }
}