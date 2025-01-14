using DecayEngine.DecPakLib;
using DecayEngine.ModuleSDK.Capability;
using DecayEngine.ModuleSDK.Component.Shader;
using DecayEngine.ModuleSDK.Exports.Attributes;
using DecayEngine.ModuleSDK.Exports.BaseExports.GameObject;
using DecayEngine.ModuleSDK.Exports.BaseExports.Scene;
using DecayEngine.ModuleSDK.Object;
using DecayEngine.ModuleSDK.Object.GameObject;
using DecayEngine.ModuleSDK.Object.Scene;

namespace DecayEngine.ModuleSDK.Exports.BaseExports.Shader
{
    [ScriptExportClass("Shader", "Represents a Shader Component.")]
    public class ShaderExport : ExportableManagedObject<IShader>, IComponentExport
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

        public override int Type => (int) ManagedExportType.Shader;

        [ExportCastConstructor]
        internal ShaderExport(ByReference<IShader> referencePointer) : base(referencePointer)
        {
        }

        [ExportCastConstructor]
        internal ShaderExport(IShader value) : base(value)
        {
        }
    }
}