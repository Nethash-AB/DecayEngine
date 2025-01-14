using System;
using DecayEngine.DecPakLib;
using DecayEngine.DecPakLib.Math.Matrix;
using DecayEngine.DecPakLib.Math.Vector;
using DecayEngine.ModuleSDK;
using DecayEngine.ModuleSDK.Component.Camera;
using DecayEngine.ModuleSDK.Component.ShaderProgram;
using DecayEngine.ModuleSDK.Object.FrameBuffer;
using DecayEngine.ModuleSDK.Object.Material;
using DecayEngine.OpenGL.Object.BufferStructure;
using DecayEngine.OpenGL.OpenGLInterop;

namespace DecayEngine.OpenGL.Object.FrameBuffer
{
    public abstract class FrameBuffer : IFrameBuffer
    {
        private uint _frameBufferHandle;
        private uint _vertexBufferHandle;
        private uint _vertexArrayHandle;

        private IRenderTargetMaterial _frameBufferMaterial;
        private IShaderProgram _shaderProgram;
        private Vector2 _size;

        private bool _dirtySize;

        private bool IsBoundRead
        {
            get
            {
                if (!Active) return false;

                return GameEngine.RenderEngine.ActiveFrameBufferRead == this;
            }
        }

        private bool IsBoundWrite
        {
            get
            {
                if (!Active) return false;

                return GameEngine.RenderEngine.ActiveFrameBufferWrite == this;
            }
        }

        public bool Destroyed { get; private set; }
        public string Name { get; set; }

        public bool Active
        {
            get => _frameBufferHandle > 0;
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

        public Vector2 Size
        {
            get => _size;
            set
            {
                if (value == _size) return;

                _size = value;
                _dirtySize = true;
            }
        }

        public IShaderProgram ShaderProgram
        {
            get => _shaderProgram;
            set => _shaderProgram = value;
        }
        public ByReference<IShaderProgram> ShaderProgramByRef => () => ref _shaderProgram;

        public IRenderTargetMaterial FrameBufferMaterial
        {
            get => _frameBufferMaterial;
            protected set => _frameBufferMaterial = value;
        }
        public ByReference<IRenderTargetMaterial> FrameBufferMaterialByRef => () => ref _frameBufferMaterial;

        ~FrameBuffer()
        {
            Destroy();
        }

        public void Destroy()
        {
            Unload();
            Destroyed = true;
        }

        public abstract void Draw(ICamera camera, Matrix4 viewMatrix, Matrix4 projectionMatrix);

        public void CopyDepthBuffer(IFrameBuffer target)
        {
            if (!Active || !target.Active) return;

            GameEngine.RenderEngine.EngineThread.ExecuteOnThread(() =>
            {
                IFrameBuffer previousTargetFbo = GameEngine.RenderEngine.ActiveFrameBufferWrite;
                IFrameBuffer previousSourceFbo = GameEngine.RenderEngine.ActiveFrameBufferRead;

                target.BindWriteOnly(false);
                BindReadOnly(false);

                GL.BlitFramebuffer(
                    0, 0, (int) Size.X, (int) Size.Y,
                    0, 0, (int) target.Size.X, (int) target.Size.Y,
                    ClearBufferMask.DepthBufferBit, BlitFramebufferFilter.Nearest
                );

                if (previousTargetFbo != null && previousTargetFbo != this)
                {
                    previousTargetFbo.BindWriteOnly(false);
                }

                if (previousSourceFbo != null && previousSourceFbo != this)
                {
                    previousSourceFbo.BindReadOnly(false);
                }
            });
        }

        public void Bind(bool clearBuffers)
        {
            if (!Active || IsBoundRead && IsBoundWrite) return;

            BindToTarget(FramebufferTarget.Framebuffer, clearBuffers);
        }

        public void BindReadOnly(bool clearBuffers)
        {
            if (!Active || IsBoundRead) return;

            BindToTarget(FramebufferTarget.ReadFramebuffer, clearBuffers);
        }

        public void BindWriteOnly(bool clearBuffers)
        {
            if (!Active || IsBoundWrite) return;

            BindToTarget(FramebufferTarget.DrawFramebuffer, clearBuffers);
        }

        public void Unbind()
        {
            if (!Active) return;

            GameEngine.RenderEngine.EngineThread.ExecuteOnThread(() =>
            {
                if (IsBoundWrite)
                {
                    GL.BindFramebuffer(FramebufferTarget.DrawFramebuffer, 0);
                    GameEngine.RenderEngine.ActiveFrameBufferWrite = null;
                }

                if (IsBoundRead)
                {
                    GL.BindFramebuffer(FramebufferTarget.ReadFramebuffer, 0);
                    GameEngine.RenderEngine.ActiveFrameBufferWrite = null;
                }
            });
        }

        protected void BindVao()
        {
            GameEngine.RenderEngine.EngineThread.ExecuteOnThread(() => GL.BindVertexArray(_vertexArrayHandle));
        }

        protected static void UnbindVao()
        {
            GameEngine.RenderEngine.EngineThread.ExecuteOnThread(() => GL.BindVertexArray(0));
        }

        private void BindToTarget(FramebufferTarget target, bool clearBuffers)
        {
            GameEngine.RenderEngine.EngineThread.ExecuteOnThread(() =>
            {
                GL.BindFramebuffer(target, _frameBufferHandle);

                switch (target)
                {
                    case FramebufferTarget.Framebuffer:
                        GameEngine.RenderEngine.ActiveFrameBufferRead = this;
                        GameEngine.RenderEngine.ActiveFrameBufferWrite = this;
                        break;
                    case FramebufferTarget.DrawFramebuffer:
                        GameEngine.RenderEngine.ActiveFrameBufferWrite = this;
                        break;
                    case FramebufferTarget.ReadFramebuffer:
                        GameEngine.RenderEngine.ActiveFrameBufferRead = this;
                        break;
                }

                if (_dirtySize)
                {
                    FrameBufferMaterial.ReloadTextures(Size);
                    _dirtySize = false;
                }

                if (clearBuffers)
                {
                    ClearBuffers();
                }
            });
        }

        private void Load()
        {
            GameEngine.RenderEngine.EngineThread.ExecuteOnThread(() =>
            {
                _frameBufferHandle = GL.GenFramebuffer();
                GL.BindFramebuffer(FramebufferTarget.Framebuffer, _frameBufferHandle);

                FrameBufferMaterial.ReloadTextures(Size);

                Bind(false);

                if (GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer) != FramebufferErrorCode.FramebufferComplete)
                {
                    throw new Exception("Frame Buffer is not complete.");
                }

                FrameBufferMaterial.AttachColorComponents();

                Unbind();

                GenerateFrameBufferQuad();
            });
        }

        private void Unload()
        {
            GameEngine.RenderEngine.EngineThread.ExecuteOnThread(() =>
            {
                Unbind();

                GL.DeleteFramebuffer(_frameBufferHandle);
                _frameBufferHandle = 0;

                FrameBufferMaterial.Destroy();
                FrameBufferMaterial = null;
            });
        }

        private void GenerateFrameBufferQuad()
        {
            _vertexArrayHandle = GL.GenVertexArray();
            _vertexBufferHandle = GL.GenBuffer();

            GL.BindVertexArray(_vertexArrayHandle);
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferHandle);

            Vertex[] quadVertices = GetVerticesForFrameBufferQuad();

            GL.BufferData(BufferTarget.ArrayBuffer, Vertex.Size * quadVertices.Length, ref quadVertices, BufferUsageHint.StaticDraw);

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

                GL.BindVertexBuffer(0, _vertexBufferHandle, IntPtr.Zero, new IntPtr(Vertex.Size));
            }

            GL.BindVertexArray(0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }

        private static Vertex[] GetVerticesForFrameBufferQuad()
        {
            return new[]
            {
                // Triangle 1
                new Vertex(-1f, 1f, 0f, 0f, 1f),
                new Vertex(-1f, -1f, 0f, 0f, 0f),
                new Vertex(1f, -1f, 0f, 1f, 0f),

                // Triangle 2
                new Vertex(-1f, 1f, 0f, 0f, 1f),
                new Vertex(1f, -1f, 0f, 1f, 0f),
                new Vertex(1f, 1f, 0f, 1f, 1f),
            };
        }

        private static void ClearBuffers()
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        }
    }
}