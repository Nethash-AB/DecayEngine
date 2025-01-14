using DecayEngine.DecPakLib;
using DecayEngine.DecPakLib.Resource.RootElement.Mesh;
using DecayEngine.ModuleSDK.Capability;
using DecayEngine.ModuleSDK.Component.Material;
using DecayEngine.ModuleSDK.Component.ShaderProgram;

namespace DecayEngine.ModuleSDK.Component.Mesh
{
    public interface IMesh : IDebugDrawable, IComponent<MeshResource>
    {
        IPbrMaterial Material { get; set; }
        ByReference<IPbrMaterial> MaterialByRef { get; }

        IShaderProgram ShaderProgram { get; set; }
        ByReference<IShaderProgram> ShaderProgramByRef { get; }
    }
}