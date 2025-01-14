using DecayEngine.DecPakLib;
using DecayEngine.ModuleSDK.Capability;
using DecayEngine.ModuleSDK.Component.ShaderProgram;
using DecayEngine.ModuleSDK.Exports.Attributes;
using DecayEngine.ModuleSDK.Exports.BaseExports.GameObject;
using DecayEngine.ModuleSDK.Exports.BaseExports.Scene;
using DecayEngine.ModuleSDK.Object;
using DecayEngine.ModuleSDK.Object.GameObject;
using DecayEngine.ModuleSDK.Object.Scene;

namespace DecayEngine.ModuleSDK.Exports.BaseExports.ShaderProgram
{
    [ScriptExportClass("ShaderProgram", "Represents a Shader Program Component.")]
    public class ShaderProgramExport : ExportableManagedObject<IShaderProgram>, IComponentExport
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

        public override int Type => (int) ManagedExportType.ShaderProgram;

        [ExportCastConstructor]
        internal ShaderProgramExport(ByReference<IShaderProgram> referencePointer) : base(referencePointer)
        {
        }

        [ExportCastConstructor]
        internal ShaderProgramExport(IShaderProgram value) : base(value)
        {
        }
    }
}