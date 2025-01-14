using DecayEngine.DecPakLib;
using DecayEngine.ModuleSDK.Capability;
using DecayEngine.ModuleSDK.Component.Script;
using DecayEngine.ModuleSDK.Exports.Attributes;
using DecayEngine.ModuleSDK.Exports.BaseExports.GameObject;
using DecayEngine.ModuleSDK.Exports.BaseExports.Scene;
using DecayEngine.ModuleSDK.Object;
using DecayEngine.ModuleSDK.Object.GameObject;
using DecayEngine.ModuleSDK.Object.Scene;

namespace DecayEngine.ModuleSDK.Exports.BaseExports.Script
{
    [ScriptExportClass("Script", "Represents a Script Component.")]
    public class ScriptExport : ExportableManagedObject<IScript>, IComponentExport
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

        public override int Type => (int) ManagedExportType.Script;

        [ScriptExportMethod("Gets a scripting environment property of the script.")]
        [return: ScriptExportReturn("The value of the specified property " +
        "or the `undefined` equivalent of the current scripting environment if the property doesn't exist.")]
        public object GetProperty(
            [ScriptExportParameter("The name of the property to retrieve the value of.")] string propertyName
        )
        {
            return Reference.GetProperty(propertyName);
        }

        [ScriptExportMethod("Sets a scripting environment property of the script." +
        "\nThe property must exist at the time of the invocation of this method and its value must not be" +
        "the `undefined` equivalent of the current scripting environment.")]
        public void SetProperty(
            [ScriptExportParameter("The name of the property to set the value of.")] string propertyName,
            [ScriptExportParameter("The value to set the property to.")] object value
        )
        {
            Reference.SetProperty(propertyName, value);
        }

        [ExportCastConstructor]
        internal ScriptExport(ByReference<IScript> referencePointer) : base(referencePointer)
        {
        }

        [ExportCastConstructor]
        internal ScriptExport(IScript value) : base(value)
        {
        }
    }
}