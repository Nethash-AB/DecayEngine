using System.Collections.Generic;
using System.IO;
using Assimp;
using DecayEngine.DecPakLib;
using DecayEngine.DecPakLib.DataStructure.Mesh;
using DecayEngine.DecPakLib.Math.Vector;
using DecayEngine.DecPakLib.Pointer;
using DecayEngine.DecPakLib.Resource;
using DecayEngine.DecPakLib.Resource.RootElement.Mesh;
using DecayEngine.DecPakLib.Resource.Structure.Math;
using Matrix4x4 = Assimp.Matrix4x4;

namespace DecayEngine.ResourceBuilderLib.Resource.Compilers.RootElement.Mesh
{
    public class MeshCompiler : IResourceCompiler<MeshResource>
    {
        public Stream Compile(IResource resource, ByReference<DataPointer> dataPointer, Stream sourceStream, out List<ByReference<DataPointer>> extraPointers)
        {
            extraPointers = new List<ByReference<DataPointer>>();
            if (!(resource is MeshResource specificResource)) return null;
            return Compile(specificResource, dataPointer, sourceStream, out extraPointers);
        }

        public Stream Decompile(IResource resource, DataPointer dataPointer, Stream sourceStream, out List<DataPointer> extraPointers)
        {
            extraPointers = new List<DataPointer>();
            if (!(resource is MeshResource specificResource)) return null;
            return Decompile(specificResource, dataPointer, sourceStream, out extraPointers);
        }

        public Stream Compile(MeshResource resource, ByReference<DataPointer> dataPointer, Stream sourceStream,
            out List<ByReference<DataPointer>> extraPointers)
        {
            extraPointers = new List<ByReference<DataPointer>>();
            return sourceStream;
        }

        public Stream Decompile(MeshResource resource, DataPointer dataPointer, Stream sourceStream, out List<DataPointer> extraPointers)
        {
            extraPointers = new List<DataPointer>();

            MeshDataStructure meshDataStructure = new MeshDataStructure();
            meshDataStructure.Deserialize(sourceStream);

            Scene scene = new Scene
            {
                RootNode = new Node(),
                Materials =
                {
                    new Material
                    {
                        Name = "defaultMaterial"
                    }
                }
            };

            Assimp.Mesh sceneMesh = new Assimp.Mesh(resource.Id, PrimitiveType.Triangle);
            for (int i = 0; i < meshDataStructure.VertexPositions.Count; i++)
            {
                Vector3 position = meshDataStructure.VertexPositions[i];
                sceneMesh.Vertices.Add(new Vector3D(position.X, position.Y, position.Z));

                Vector3 normal = Vector3.Zero;
                if (meshDataStructure.VertexNormals != null && meshDataStructure.VertexNormals.Count > i)
                {
                    normal = meshDataStructure.VertexNormals[i];
                }
                sceneMesh.Normals.Add(new Vector3D(normal.X, normal.Y, normal.Z));

                Vector3 tangent = Vector3.Zero;
                if (meshDataStructure.VertexTangents != null && meshDataStructure.VertexTangents.Count > i)
                {
                    tangent = meshDataStructure.VertexTangents[i];
                }
                sceneMesh.Tangents.Add(new Vector3D(tangent.X, tangent.Y, tangent.Z));

                Vector3 bitangent = Vector3.Zero;
                if (meshDataStructure.VertexBitangents != null && meshDataStructure.VertexBitangents.Count > i)
                {
                    bitangent = meshDataStructure.VertexBitangents[i];
                }
                sceneMesh.BiTangents.Add(new Vector3D(bitangent.X, bitangent.Y, bitangent.Z));

                Vector2 textureUv = Vector2.Zero;
                if (meshDataStructure.VertexUvCoordinates != null && meshDataStructure.VertexUvCoordinates.Count > i)
                {
                    textureUv = meshDataStructure.VertexUvCoordinates[i];
                }
                sceneMesh.TextureCoordinateChannels[0].Add(new Vector3D(textureUv.X, textureUv.Y, 0f));
            }

            foreach (TriangleStructure triangleStructure in meshDataStructure.Triangles)
            {
                sceneMesh.Faces.Add(new Face
                {
                    Indices =
                    {
                        triangleStructure.Vertex1,
                        triangleStructure.Vertex2,
                        triangleStructure.Vertex3
                    }
                });
            }

            sceneMesh.MaterialIndex = 0;

            scene.Meshes.Add(sceneMesh);

            Node meshNode = new Node
            {
                Transform = Matrix4x4.Identity,
                MeshIndices = {0}
            };

            scene.RootNode.Children.Add(meshNode);

            MemoryStream ms = new MemoryStream();
            using (AssimpContext context = new AssimpContext())
            {
                ExportDataBlob blob = context.ExportToBlob(scene, "obj");
                ms.Write(blob.Data, 0, blob.Data.Length);
            }

            dataPointer.FullSourcePath =
                $"{Path.GetDirectoryName(dataPointer.FullSourcePath)}" +
                $"{Path.DirectorySeparatorChar}" +
                $"{Path.GetFileNameWithoutExtension(dataPointer.FullSourcePath)}.obj";

            return ms;
        }
    }
}