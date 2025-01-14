using System;
using DecayEngine.DecPakLib;
using DecayEngine.DecPakLib.DataStructure.Mesh;
using DecayEngine.DecPakLib.Math.Matrix;
using DecayEngine.DecPakLib.Math.Vector;
using DecayEngine.DecPakLib.Resource.RootElement.Mesh;
using DecayEngine.DecPakLib.Resource.Structure.Math;
using DecayEngine.ModuleSDK;
using DecayEngine.ModuleSDK.Capability;
using DecayEngine.ModuleSDK.Component.Camera;
using DecayEngine.ModuleSDK.Component.Material;
using DecayEngine.ModuleSDK.Component.Mesh;
using DecayEngine.ModuleSDK.Component.ShaderProgram;
using DecayEngine.ModuleSDK.Logging;
using DecayEngine.ModuleSDK.Object.GameObject;
using DecayEngine.ModuleSDK.Object.Scene;
using DecayEngine.ModuleSDK.Object.Transform;
using DecayEngine.OpenGL.Object.BufferStructure;
using DecayEngine.OpenGL.OpenGLInterop;

namespace DecayEngine.OpenGL.Component.Mesh
{
    public class MeshComponent : IMesh
    {
        private IGameObject _parent;

        private IPbrMaterial _material;
        private IShaderProgram _shaderProgram;
        private readonly MeshDataStructure _meshDataStructure;

        private uint _vertexBufferHandle;
        private uint _vertexElementBufferHandle;
        private uint _vertexArrayHandle;

        private int _shouldDrawFlag;
        private bool _isBuffering;
        private Vector3 _drawableSize;
        private Vector4 _debugColor;

        public IGameObject Parent => _parent;
        public ByReference<IGameObject> ParentByRef => () => ref _parent;

        public MeshResource Resource { get; set; }
        public Type ExportType => null;

        public bool Destroyed { get; private set; }
        public string Name { get; set; }

        public bool Active
        {
            get => Parent != null && _vertexArrayHandle > 0;
            set
            {
                if (!Active && value)
                {
                    Load();
                }
                else if (Active && !value)
                {
                    Unload();
                }
            }
        }

        public IPbrMaterial Material
        {
            get => _material;
            set => _material = value;
        }
        public ByReference<IPbrMaterial> MaterialByRef => () => ref _material;

        public IShaderProgram ShaderProgram
        {
            get => _shaderProgram;
            set => _shaderProgram = value;
        }
        public ByReference<IShaderProgram> ShaderProgramByRef => () => ref _shaderProgram;

        public Vector3 Pivot { get; }
        public Vector3 DrawableSize => _drawableSize;
        public bool ShouldDraw
        {
            get => _shouldDrawFlag != 0;
            set => _shouldDrawFlag = value ? 1 : 0;
        }

        public Transform WorldSpaceTransform => Parent.WorldSpaceTransform;
        public bool IsPbrCapable => _material != null;

        public MeshComponent(MeshDataStructure meshDataStructure)
        {
            _shouldDrawFlag = -1;
            _meshDataStructure = meshDataStructure;

            Pivot = Vector3.Zero;

            Random rand = new Random();
            Vector3 color = new Vector3((float) rand.NextDouble(), (float) rand.NextDouble(), (float) rand.NextDouble()).Normalized;
            _debugColor = new Vector4(color, 1f);
        }

        ~MeshComponent()
        {
            Destroy();
        }

        public void SetParent(IGameObject parent)
        {
            _parent?.RemoveComponent(this);

            if (Material?.AsParentable<IGameObject>().Parent == null)
            {
                Material?.SetParent(parent);
            }
            if (ShaderProgram?.AsParentable<IGameObject>().Parent == null && ShaderProgram?.AsParentable<IScene>().Parent == null)
            {
                ShaderProgram?.SetParent(parent);
            }

            parent?.AttachComponent(this);
            _parent = parent;
        }

        public IParentable<IGameObject> AsParentable<T>() where T : IGameObject
        {
            return this;
        }

        public void Draw(Matrix4 viewMatrix, Matrix4 projectionMatrix)
        {
            if (!ShouldDraw || _isBuffering) return;

            if (ShaderProgram == null || !ShaderProgram.Active)
            {
                GameEngine.LogAppendLine(LogSeverity.Warning,
                    "OpenGL",
                    $"Tried to draw mesh ({Name}) without a shader program. Deactivating sprite component."
                );
                Active = false;
                return;
            }

            if (Material == null || !Material.Active)
            {
                GameEngine.LogAppendLine(LogSeverity.Warning,
                    "OpenGL",
                    $"Tried to draw mesh ({Name}) without a material. Deactivating sprite component."
                );
                Active = false;
                return;
            }

            ShaderProgram.Bind();
            ShaderProgram.SetVariable(OpenGlConstants.Uniforms.View, viewMatrix);
            ShaderProgram.SetVariable(OpenGlConstants.Uniforms.Projection, projectionMatrix);
            ShaderProgram.SetVariable(OpenGlConstants.Uniforms.Model, WorldSpaceTransform.TransformMatrix);

            ShaderProgram.SetVariable(OpenGlConstants.Uniforms.AlbedoColor, Material.AlbedoColor);
            ShaderProgram.SetVariable(OpenGlConstants.Uniforms.MetallicityFactor, Material.MetallicityFactor);
            ShaderProgram.SetVariable(OpenGlConstants.Uniforms.RoughnessFactor, Material.RoughnessFactor);
            ShaderProgram.SetVariable(OpenGlConstants.Uniforms.EmissionColor, Material.EmissionColor);

            ShaderProgram.SetVariable(
                OpenGlConstants.Uniforms.UseAlbedoMap,
                Material.AlbedoTexture != null && Material.AlbedoTexture.Active ? 1 : 0
            );
            ShaderProgram.SetVariable(
                OpenGlConstants.Uniforms.UseMetallicityMap,
                Material.MetallicityTexture != null && Material.MetallicityTexture.Active ? 1 : 0
            );
            ShaderProgram.SetVariable(
                OpenGlConstants.Uniforms.UseRoughnessMap,
                Material.RoughnessTexture != null && Material.RoughnessTexture.Active ? 1 : 0
            );
            ShaderProgram.SetVariable(
                OpenGlConstants.Uniforms.UseEmissionMap,
                Material.EmissionTexture != null && Material.EmissionTexture.Active ? 1 : 0
            );

            GL.BindVertexArray(_vertexArrayHandle);

            Material.Bind();

            GL.DrawElements(BeginMode.Triangles, _meshDataStructure.Triangles.Count * 3, DrawElementsType.UnsignedShort, IntPtr.Zero);

            Material.Unbind();

            GL.BindVertexArray(0);

            ShaderProgram.Unbind();
        }

        public void DrawDebug(Matrix4 viewMatrix, Matrix4 projectionMatrix, IDebugDrawer debugDrawer)
        {
            if (!ShouldDraw || _isBuffering || !debugDrawer.DebugGeometryShaderProgram.Active) return;

            GameEngine.RenderEngine.WireFrameEnabled = true;

            debugDrawer.DebugGeometryShaderProgram.Bind();
            debugDrawer.DebugGeometryShaderProgram.SetVariable(OpenGlConstants.Uniforms.View, viewMatrix);
            debugDrawer.DebugGeometryShaderProgram.SetVariable(OpenGlConstants.Uniforms.Projection, projectionMatrix);
            debugDrawer.DebugGeometryShaderProgram.SetVariable(OpenGlConstants.Uniforms.Model, WorldSpaceTransform.TransformMatrix);
            debugDrawer.DebugGeometryShaderProgram.SetVariable(OpenGlConstants.Uniforms.Color, _debugColor);

            GL.BindVertexArray(_vertexArrayHandle);

            GL.DrawElements(BeginMode.Triangles, _meshDataStructure.Triangles.Count * 3, DrawElementsType.UnsignedShort, IntPtr.Zero);

            GL.BindVertexArray(0);

            debugDrawer.DebugGeometryShaderProgram.Unbind();

            GameEngine.RenderEngine.WireFrameEnabled = false;
        }

        public void Destroy()
        {
            Unload();

            _shaderProgram = null;

            SetParent(null);

            Destroyed = true;
        }

        private void Load()
        {
            if (ShaderProgram == null)
            {
                throw new Exception("No shader program.");
            }

            GameEngine.RenderEngine.EngineThread.ExecuteOnThread(() =>
            {
                _vertexArrayHandle = GL.GenVertexArray();
                _vertexBufferHandle = GL.GenBuffer();
                _vertexElementBufferHandle = GL.GenBuffer();

                BufferVertices(true);

                if (GameEngine.RenderEngine.IsEmbedded)
                {
                    // Position
                    GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false,
                        VertexPbr.Size, IntPtr.Zero);
                    GL.EnableVertexAttribArray(0);

                    // UV
                    GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false,
                        VertexPbr.Size, new IntPtr(4 * (uint) Vector3.SizeInBytes));
                    GL.EnableVertexAttribArray(1);

                    // Normal
                    GL.VertexAttribPointer(2, 3, VertexAttribPointerType.Float, false,
                        VertexPbr.Size, new IntPtr((uint) Vector3.SizeInBytes));
                    GL.EnableVertexAttribArray(2);

                    // Tangent
                    GL.VertexAttribPointer(3, 3, VertexAttribPointerType.Float, false,
                        VertexPbr.Size, new IntPtr(2 * (uint) Vector3.SizeInBytes));
                    GL.EnableVertexAttribArray(3);

                    // Bitangent
                    GL.VertexAttribPointer(4, 3, VertexAttribPointerType.Float, false,
                        VertexPbr.Size, new IntPtr(3 * (uint) Vector3.SizeInBytes));
                    GL.EnableVertexAttribArray(4);
                }
                else
                {
                    // Position
                    GL.VertexAttribBinding(0, 0);
                    GL.EnableVertexAttribArray(0);
                    GL.VertexAttribFormat(0, 3, VertexAttribFormat.Float, false, 0);

                    // UV
                    GL.VertexAttribBinding(1, 0);
                    GL.EnableVertexAttribArray(1);
                    GL.VertexAttribFormat(1, 2, VertexAttribFormat.Float, false, 4 * (uint) Vector3.SizeInBytes);

                    // Normal
                    GL.VertexAttribBinding(2, 0);
                    GL.EnableVertexAttribArray(2);
                    GL.VertexAttribFormat(2, 3, VertexAttribFormat.Float, false, (uint) Vector3.SizeInBytes);

                    // Tangent
                    GL.VertexAttribBinding(3, 0);
                    GL.EnableVertexAttribArray(3);
                    GL.VertexAttribFormat(3, 3, VertexAttribFormat.Float, false, 2 * (uint) Vector3.SizeInBytes);

                    // Bitangent
                    GL.VertexAttribBinding(4, 0);
                    GL.EnableVertexAttribArray(4);
                    GL.VertexAttribFormat(4, 3, VertexAttribFormat.Float, false, 3 * (uint) Vector3.SizeInBytes);

                    GL.BindVertexBuffer(0, _vertexBufferHandle, IntPtr.Zero, new IntPtr(VertexPbr.Size));
                }

                GL.BindVertexArray(0);
                GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
            });
        }

        private void Unload()
        {
            if (_vertexArrayHandle < 1) return;

            _isBuffering = true;
            GameEngine.RenderEngine.EngineThread.ExecuteOnThread(() =>
            {
                GL.DeleteVertexArray(_vertexArrayHandle);
                _vertexArrayHandle = 0;
                GL.DeleteBuffer(_vertexBufferHandle);
                _vertexBufferHandle = 0;
                GL.DeleteBuffer(_vertexElementBufferHandle);
                _vertexElementBufferHandle = 0;

                _isBuffering = false;
            });
        }

        private void BufferVertices(bool leaveOpen = false)
        {
            if (_isBuffering) return;

            _isBuffering = true;
            GameEngine.RenderEngine.EngineThread.ExecuteOnThread(() =>
            {
                GL.BindVertexArray(_vertexArrayHandle);

                GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferHandle);
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, _vertexElementBufferHandle);

                if (_meshDataStructure.VertexPositions == null || _meshDataStructure.Triangles == null)
                {
                    return;
                }

                VertexPbr[] vertices = new VertexPbr[_meshDataStructure.VertexPositions.Count];
                short[] indices = new short[_meshDataStructure.Triangles.Count * 3];

                for (int i = 0; i < vertices.Length; i++)
                {
                    Vector3 position = _meshDataStructure.VertexPositions[i];

                    Vector3 normal = Vector3.Zero;
                    if (_meshDataStructure.VertexNormals != null && _meshDataStructure.VertexNormals.Count > i)
                    {
                        normal = _meshDataStructure.VertexNormals[i];
                    }

                    Vector3 tangent = Vector3.Zero;
                    if (_meshDataStructure.VertexTangents != null && _meshDataStructure.VertexTangents.Count > i)
                    {
                        tangent = _meshDataStructure.VertexTangents[i];
                    }

                    Vector3 bitangent = Vector3.Zero;
                    if (_meshDataStructure.VertexBitangents != null && _meshDataStructure.VertexBitangents.Count > i)
                    {
                        bitangent = _meshDataStructure.VertexBitangents[i];
                    }

                    Vector2 textureUv = Vector2.Zero;
                    if (_meshDataStructure.VertexUvCoordinates != null && _meshDataStructure.VertexUvCoordinates.Count > i)
                    {
                        textureUv = new Vector2(_meshDataStructure.VertexUvCoordinates[i].X, 1f - _meshDataStructure.VertexUvCoordinates[i].Y);
                    }

                    vertices[i] = new VertexPbr(position, normal, tangent, bitangent, textureUv);
                }

                int j = 0;
                foreach (TriangleStructure triangleStructure in _meshDataStructure.Triangles)
                {
                    indices[j] = (short) triangleStructure.Vertex1;
                    indices[j + 1] = (short) triangleStructure.Vertex2;
                    indices[j + 2] = (short) triangleStructure.Vertex3;

                    j += 3;
                }

                GL.BufferData(BufferTarget.ArrayBuffer, VertexPbr.Size * vertices.Length, ref vertices, BufferUsageHint.StaticDraw);
                GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(short), ref indices, BufferUsageHint.StaticDraw);
            });

            if (leaveOpen)
            {
                _isBuffering = false;
                return;
            }

            GL.BindVertexArray(0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);

            _isBuffering = false;
        }
    }
}