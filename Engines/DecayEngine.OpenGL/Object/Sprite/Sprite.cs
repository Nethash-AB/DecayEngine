using System;
using DecayEngine.DecPakLib;
using DecayEngine.DecPakLib.Math.Matrix;
using DecayEngine.DecPakLib.Math.Vector;
using DecayEngine.ModuleSDK;
using DecayEngine.ModuleSDK.Capability;
using DecayEngine.ModuleSDK.Component.Camera;
using DecayEngine.ModuleSDK.Component.ShaderProgram;
using DecayEngine.ModuleSDK.Object.Sprite;
using DecayEngine.ModuleSDK.Object.Transform;
using DecayEngine.OpenGL.Object.BufferStructure;
using DecayEngine.OpenGL.OpenGLInterop;

namespace DecayEngine.OpenGL.Object.Sprite
{
    public abstract class Sprite : ISprite
    {
        private Transform _transform;
        private IShaderProgram _shaderProgram;

        protected uint VertexBufferHandle;
        protected uint VertexElementBufferHandle;
        protected uint VertexArrayHandle;

        protected int ShouldDrawFlag;
        protected bool IsBuffering;
        protected Vector3 DrawableSizeInternal;
        protected Vector4 DebugColor;

        protected Vector2 CurrentAspectRatio;

        public bool Destroyed { get; private set; }
        public string Name { get; set; }

        public virtual bool Active
        {
            get => VertexArrayHandle > 0;
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

        public Transform Transform => _transform;
        public ByReference<Transform> TransformByRef => () => ref _transform;
        public Transform WorldSpaceTransform => this.GetWorldSpaceTransform();
        public bool IsPbrCapable => false;

        public IShaderProgram ShaderProgram
        {
            get => _shaderProgram;
            set => _shaderProgram = value;
        }
        public ByReference<IShaderProgram> ShaderProgramByRef => () => ref _shaderProgram;

        public bool MaintainAspectRatio { get; set; }

        public Vector3 Pivot { get; }
        public Vector3 DrawableSize => DrawableSizeInternal * Transform.Scale;

        public bool ShouldDraw
        {
            get => ShouldDrawFlag != 0;
            set => ShouldDrawFlag = value ? 1 : 0;
        }

        protected Sprite()
        {
            ShouldDrawFlag = -1;
            _transform = new Transform();

            Pivot = Vector3.Zero;

            Random rand = new Random();
            Vector3 color = new Vector3((float) rand.NextDouble(), (float) rand.NextDouble(), (float) rand.NextDouble()).Normalized;
            DebugColor = new Vector4(color, 1f);
        }

        ~Sprite()
        {
            Destroy();
        }

        public abstract void Draw(Matrix4 viewMatrix, Matrix4 projectionMatrix);
        public abstract void DrawDebug(Matrix4 viewMatrix, Matrix4 projectionMatrix, IDebugDrawer debugDrawer);

        public virtual void Destroy()
        {
            Unload();

            _shaderProgram = null;
            _transform = null;

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
                VertexArrayHandle = GL.GenVertexArray();
                VertexBufferHandle = GL.GenBuffer();
                VertexElementBufferHandle = GL.GenBuffer();

                BufferVertices(true);

                if (GameEngine.RenderEngine.IsEmbedded)
                {
                    // Position
                    GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, Vertex.Size, IntPtr.Zero);
                    GL.EnableVertexAttribArray(0);

                    // UV
                    GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, Vertex.Size, new IntPtr(3 * sizeof(float)));
                    GL.EnableVertexAttribArray(1);
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
                    GL.VertexAttribFormat(1, 2, VertexAttribFormat.Float, false, 3 * sizeof(float));

                    GL.BindVertexBuffer(0, VertexBufferHandle, IntPtr.Zero, new IntPtr(Vertex.Size));
                }

                GL.BindVertexArray(0);
                GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
            });
        }

        private void Unload()
        {
            if (VertexArrayHandle < 1) return;

            IsBuffering = true;
            GameEngine.RenderEngine.EngineThread.ExecuteOnThread(() =>
            {
                GL.DeleteVertexArray(VertexArrayHandle);
                VertexArrayHandle = 0;
                GL.DeleteBuffer(VertexBufferHandle);
                VertexBufferHandle = 0;
                GL.DeleteBuffer(VertexElementBufferHandle);
                VertexElementBufferHandle = 0;

                IsBuffering = false;
            });
        }

        protected void BufferVertices(bool leaveOpen = false)
        {
            if (IsBuffering) return;

            IsBuffering = true;
            GameEngine.RenderEngine.EngineThread.ExecuteOnThread(() =>
            {
                GL.BindVertexArray(VertexArrayHandle);

                GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBufferHandle);
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, VertexElementBufferHandle);

                CalculateVertices(out Vertex[] vertices, out short[] indices);

                GL.BufferData(BufferTarget.ArrayBuffer, Vertex.Size * vertices.Length, ref vertices, BufferUsageHint.StaticDraw);
                GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(short), ref indices, BufferUsageHint.StaticDraw);

                if (leaveOpen)
                {
                    IsBuffering = false;
                    return;
                }

                GL.BindVertexArray(0);
                GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);

                IsBuffering = false;
            });
        }

        protected abstract void CalculateVertices(out Vertex[] vertices, out short[] indices);
    }
}