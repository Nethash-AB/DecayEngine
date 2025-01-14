using System;
using System.Collections.Generic;
using System.IO;
using Autodesk.Maya.OpenMaya;
using DecayEngine.DecPakLib.DataStructure;
using DecayEngine.DecPakLib.DataStructure.Mesh;
using DecayEngine.DecPakLib.Pointer;
using DecayEngine.DecPakLib.Resource;
using DecayEngine.DecPakLib.Resource.Expression.Property.Reference;
using DecayEngine.DecPakLib.Resource.Expression.Query.Collection;
using DecayEngine.DecPakLib.Resource.Expression.Query.Collection.Filter;
using DecayEngine.DecPakLib.Resource.Expression.Query.Collection.Terminator;
using DecayEngine.DecPakLib.Resource.Expression.Query.Initiator;
using DecayEngine.DecPakLib.Resource.Expression.Statement.Component.ComponentImplementations.Mesh;
using DecayEngine.DecPakLib.Resource.RootElement.Mesh;
using DecayEngine.DecPakLib.Resource.Structure.Component;
using DecayEngine.DecPakLib.Resource.Structure.Math;
using DecayEngine.MayaPlugin.MayaInterop;

namespace DecayEngine.MayaPlugin.Scene.Node
{
    public class MayaMeshNode : IMayaNode
    {
        public MayaScene Scene { get; set; }
        public MObject MayaObject { get; }
        public List<IMayaNode> Children { get; }

        public MayaMeshNode(MObject mayaObject)
        {
            MayaObject = mayaObject;
            Children = new List<IMayaNode>();
        }

        public string DisplayId
        {
            get
            {
                MFnDagNode dagNodeFn = new MFnDagNode(MayaObject);
                return $"{dagNodeFn.name} [{GetType().Name}({dagNodeFn.typeName})]";
            }
        }

        public bool IsValid => Children.Count < 1;

        public IEnumerable<IResource> ToDecayResource(Uri outputDirUri)
        {
            List<IResource> resources = new List<IResource>();

            MFnMesh meshNodeFn = new MFnMesh(MayaObject);

            // Per mesh raw data
            MFloatPointArray meshPoints = new MFloatPointArray();
            meshNodeFn.getPoints(meshPoints, MSpace.Space.kObject);
//            MGlobal.displayInfo($"Mesh #{meshNodeFn.name} has '{meshPoints.Count}' points.\n");

            MFloatVectorArray meshNormals = new MFloatVectorArray();
            meshNodeFn.getNormals(meshNormals, MSpace.Space.kObject);
//            MGlobal.displayInfo($"Mesh #{meshNodeFn.name} has '{meshNormals.Count}' normals.\n");

            MFloatVectorArray meshTangents = new MFloatVectorArray();
            meshNodeFn.getTangents(meshTangents, MSpace.Space.kObject);

            MFloatVectorArray meshBinormals = new MFloatVectorArray();
            meshNodeFn.getBinormals(meshBinormals, MSpace.Space.kObject);

            MFloatArray uCoordinates = new MFloatArray();
            MFloatArray vCoordinates = new MFloatArray();
            meshNodeFn.getUVs(uCoordinates, vCoordinates);

            List<Vertex> vertices = new List<Vertex>();
            for (MItMeshPolygon polygonIterator = new MItMeshPolygon(MayaObject); !polygonIterator.isDone; polygonIterator.next())
            {
                if (!polygonIterator.hasValidTriangulation)
                {
                    throw new Exception($"Polygon #{polygonIterator.index()} has invalid triangulation.");
                }

                MIntArray polygonVertices = new MIntArray();
                polygonIterator.getVertices(polygonVertices);

                polygonIterator.numTriangles(out int polygonTriangleCount);
                for (int i = 0; i < polygonTriangleCount; i++)
                {
                    // Triangles
                    MPointArray trianglePoints = new MPointArray();
                    MIntArray triangleIndices = new MIntArray();
                    polygonIterator.getTriangle(i, trianglePoints, triangleIndices, MSpace.Space.kObject);

                    // Local Indices
                    MIntArray localIndices = GetLocalIndex(polygonVertices, triangleIndices);

                    // Triangle Vertices
                    for (int j = 0; j < triangleIndices.Count; j++)
                    {
                        int index = triangleIndices[j];
                        int localIndex = localIndices[j];

                        polygonIterator.getUVIndex(localIndex, out int uvIndex);

                        int normalIndex = (int) polygonIterator.normalIndex(localIndex);
                        int tangentIndex = (int) polygonIterator.tangentIndex(localIndex);

                        vertices.Add(new Vertex
                        {
                            Id = index,
                            Position = meshPoints[index],
                            Normal = meshNormals[normalIndex],
                            Tangent = meshTangents[tangentIndex],
                            Bitangent = meshBinormals[tangentIndex],
                            UvCoordinates = new[]
                            {
                                uCoordinates[uvIndex],
                                vCoordinates[uvIndex]
                            }
                        });
                    }
                }
            }

            List<Vertex> uniqueVertices = new List<Vertex>();
            List<int> indices = new List<int>();
            foreach (Vertex vertex in vertices)
            {
                bool found = false;

                for (int j = 0; j < uniqueVertices.Count; j++)
                {
                    if (!vertex.Equals(uniqueVertices[j])) continue;

                    indices.Add(j);
                    found = true;
                    break;
                }

                if (found) continue;

                indices.Add(uniqueVertices.Count);
                uniqueVertices.Add(vertex);
            }

//            MGlobal.displayInfo($"Mesh contains {uniqueVertices.Count} unique vertices and {indices.Count} indices.\n");

            MeshDataStructure meshDataStructure = new MeshDataStructure
            {
                Name = meshNodeFn.name,
                VertexPositions = new List<Vector3Structure>(),
                VertexNormals = new List<Vector3Structure>(),
                VertexTangents = new List<Vector3Structure>(),
                VertexBitangents = new List<Vector3Structure>(),
                VertexUvCoordinates = new List<Vector2Structure>(),
                Triangles = new List<TriangleStructure>()
            };

            foreach (Vertex uniqueVertex in uniqueVertices)
            {
                meshDataStructure.VertexPositions.Add(new Vector3Structure
                {
                    X = uniqueVertex.Position.x,
                    Y = uniqueVertex.Position.y,
                    Z = uniqueVertex.Position.z
                });

                meshDataStructure.VertexNormals.Add(new Vector3Structure
                {
                    X = uniqueVertex.Normal.x,
                    Y = uniqueVertex.Normal.y,
                    Z = uniqueVertex.Normal.z
                });

                meshDataStructure.VertexTangents.Add(new Vector3Structure
                {
                    X = uniqueVertex.Tangent.x,
                    Y = uniqueVertex.Tangent.y,
                    Z = uniqueVertex.Tangent.z
                });

                meshDataStructure.VertexBitangents.Add(new Vector3Structure
                {
                    X = uniqueVertex.Bitangent.x,
                    Y = uniqueVertex.Bitangent.y,
                    Z = uniqueVertex.Bitangent.z
                });

                meshDataStructure.VertexUvCoordinates.Add(new Vector2Structure()
                {
                    X = uniqueVertex.UvCoordinates[0],
                    Y = uniqueVertex.UvCoordinates[1]
                });
            }

            for (int i = 0; i < indices.Count; i += 3)
            {
                meshDataStructure.Triangles.Add(new TriangleStructure
                {
                    Vertex1 = indices[i],
                    Vertex2 = indices[i + 1],
                    Vertex3 = indices[i + 2]
                });
            }

            string materialResourceId = null;
            MObjectArray shaders = new MObjectArray();
            MIntArray shaderIndices = new MIntArray();
            meshNodeFn.getConnectedShaders(0, shaders, shaderIndices);
            if (shaders.Count > 0)
            {
                MFnDependencyNode shadingEngineDependencyNode = new MFnDependencyNode(shaders[0]);

                MPlug surfaceShaderPlug = shadingEngineDependencyNode.findPlug("surfaceShader");
                MPlugArray materialPlugs = new MPlugArray();
                surfaceShaderPlug.connectedTo(materialPlugs, true, false);

                if (materialPlugs.Count > 0)
                {
                    MObject materialObject = materialPlugs[0].node;
                    MFnDependencyNode materialDependencyNode = new MFnDependencyNode(materialObject);
                    materialResourceId = materialDependencyNode.name;
//                    if (materialObject.hasFn(MFn.Type.kBlinn))
//                    {
//                        MFnBlinnShader blinnMaterial = new MFnBlinnShader(materialObject);
//                        materialResourceId = blinnMaterial.name;
//                    }
                }
            }

            Uri meshDataOutputDirUri = new Uri(outputDirUri, new Uri($"meshes{Path.DirectorySeparatorChar}", UriKind.Relative));
            MeshResource meshResource = ExportDecayMesh(meshDataStructure, meshDataOutputDirUri);

            CreateMeshComponentExpression meshComponentExpression = GenerateMeshComponentExpression(meshResource.Id, materialResourceId);

            resources.Add(meshResource);
            resources.Add(meshComponentExpression);

            return resources;
        }

        private static MIntArray GetLocalIndex(MIntArray vertices, MIntArray indices)
        {
            MIntArray localIndex = new MIntArray();

            foreach (int i in indices)
            {
                for (int j = 0; j < vertices.Count; j++)
                {
                    if (i != vertices[j]) continue;

                    localIndex.append(j);
                    break;
                }
            }

            return localIndex;
        }

        private MeshResource ExportDecayMesh(MeshDataStructure meshDataStructure, Uri outputDirUri)
        {
            if (!Directory.Exists(outputDirUri.LocalPath))
            {
                Directory.CreateDirectory(outputDirUri.LocalPath);
            }

            string meshFileName = $"{meshDataStructure.Name}.dec3d";
            Uri meshDataUri = new Uri(outputDirUri, new Uri(meshFileName, UriKind.Relative));
            string meshDataPath = meshDataUri.LocalPath;

            MGlobal.displayInfo($"Serializing Mesh ({meshDataStructure.Name}) to file: {meshDataPath}. Stats:");
            MGlobal.displayInfo("{\n" +
                $"  Vertex position count: {meshDataStructure.VertexPositions.Count},\n" +
                $"  Vertex normal count: {meshDataStructure.VertexPositions.Count},\n" +
                $"  Vertex tangent count: {meshDataStructure.VertexTangents.Count},\n" +
                $"  Vertex bitangent count: {meshDataStructure.VertexBitangents.Count},\n" +
                $"  Vertex uv coordinate count: {meshDataStructure.VertexUvCoordinates.Count},\n" +
                $"  Triangle count: {meshDataStructure.Triangles.Count}\n" +
                "}");

            using (MemoryStream ms = meshDataStructure.Serialize())
            {
                if (File.Exists(meshDataPath))
                {
                    File.Delete(meshDataPath);
                }

                using (FileStream fs = File.OpenWrite(meshDataPath))
                {
                    ms.Position = 0;
                    ms.CopyTo(fs);
                }
            }

            return new MeshResource
            {
                Id = meshDataStructure.Name,
                MetaFilePath = $"{Path.GetFileNameWithoutExtension(Scene.MetaFileName)}_meshes.meta",
                Source = new DataPointer
                {
                    SourcePath = $"./meshes/{meshFileName}"
                }
            };
        }

        private static CreateMeshComponentExpression GenerateMeshComponentExpression(string meshResourceId, string materialResourceId)
        {
            if (string.IsNullOrEmpty(meshResourceId) || string.IsNullOrEmpty(materialResourceId))
            {
                return null;
            }

            return new CreateMeshComponentExpression
            {
                Name = $"mesh_{meshResourceId}",
                Active = true,
                Template = new ResourceReferenceExpression
                {
                    ReferenceId = meshResourceId
                },
                Material = new SelectRootExpression
                {
                    Next = new SelectComponentsExpression
                    {
                        Next = new FilterByNameExpression
                        {
                            Name = $"material_{materialResourceId}",
                            Next = new SelectFirstCollectionTerminatorExpression()
                        }
                    }
                },
                ShaderProgram = new SelectRootExpression
                {
                    Next = new SelectComponentsExpression
                    {
                        Next = new FilterByNameExpression
                        {
                            Name = "default_pbr_shader_program",
                            Next = new SelectFirstCollectionTerminatorExpression()
                        }
                    }
                },
                Camera = new SelectActiveSceneExpression
                {
                    Next = new SelectComponentsExpression
                    {
                        Next = new FilterByComponentTypeExpression
                        {
                            Type = ComponentType.CameraPersp,
                            Next = new SelectFirstCollectionTerminatorExpression()
                        }
                    }
                }
            };
        }
    }
}