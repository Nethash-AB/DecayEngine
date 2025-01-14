using System.IO;
using DecayEngine.DecPakLib.DataStructure;
using DecayEngine.DecPakLib.DataStructure.Mesh;
using DecayEngine.DecPakLib.Resource.RootElement.Mesh;
using DecayEngine.ModuleSDK.Component;
using DecayEngine.ModuleSDK.Component.Mesh;

namespace DecayEngine.OpenGL.Component.Mesh
{
    public class MeshFactory : IComponentFactory<MeshComponent, MeshResource>, IComponentFactory<IMesh, MeshResource>
    {
        public MeshComponent CreateComponent(MeshResource resource)
        {
            MeshDataStructure meshDataStructure = new MeshDataStructure();

            using (MemoryStream ms = resource.Source.GetData())
            {
                meshDataStructure.Deserialize(ms);
            }

            return new MeshComponent(meshDataStructure) {Resource = resource};
        }

        IMesh IComponentFactory<IMesh, MeshResource>.CreateComponent(MeshResource resource)
        {
            return CreateComponent(resource);
        }
    }
}