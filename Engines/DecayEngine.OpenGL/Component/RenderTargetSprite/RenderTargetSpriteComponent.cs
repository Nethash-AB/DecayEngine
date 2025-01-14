using System;
using DecayEngine.DecPakLib;
using DecayEngine.DecPakLib.Math.Matrix;
using DecayEngine.DecPakLib.Math.Vector;
using DecayEngine.ModuleSDK;
using DecayEngine.ModuleSDK.Capability;
using DecayEngine.ModuleSDK.Component.Camera;
using DecayEngine.ModuleSDK.Component.Sprite;
using DecayEngine.ModuleSDK.Logging;
using DecayEngine.ModuleSDK.Object.FrameBuffer;
using DecayEngine.ModuleSDK.Object.GameObject;
using DecayEngine.ModuleSDK.Object.Material;
using DecayEngine.ModuleSDK.Object.Scene;
using DecayEngine.OpenGL.Object.BufferStructure;
using DecayEngine.OpenGL.Object.Sprite;
using DecayEngine.OpenGL.OpenGLInterop;

namespace DecayEngine.OpenGL.Component.RenderTargetSprite
{
    public class RenderTargetSpriteComponent : Sprite, IRenderTargetSprite
    {
        private IGameObject _parent;
        private ByReference<IRenderFrameBuffer> _fboByRef;
        private IRenderFrameBuffer _fboByValue;

        public IGameObject Parent => _parent;
        public ByReference<IGameObject> ParentByRef => () => ref _parent;

        public Type ExportType => null;

        public override bool Active
        {
            get => Parent != null && base.Active;
            set => base.Active = value;
        }

        public ByReference<IRenderFrameBuffer> SourceFrameBufferByRef
        {
            get => _fboByRef ?? (() => ref _fboByValue);
            set
            {
                _fboByValue = null;
                _fboByRef = value;
            }
        }

        public IRenderFrameBuffer SourceFrameBuffer
        {
            get => _fboByRef() ?? _fboByValue;
            set
            {
                _fboByRef = null;
                _fboByValue = value;
            }
        }

        public RenderTargetSpriteComponent()
        {
            CurrentAspectRatio = Vector2.One;
            DrawableSizeInternal = new Vector3(1f, 1f, 0f);
        }

        ~RenderTargetSpriteComponent()
        {
            Destroy();
        }

        public void SetParent(IGameObject parent)
        {
            _parent?.RemoveComponent(this);

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

        public override void Draw(Matrix4 viewMatrix, Matrix4 projectionMatrix)
        {
            if (!ShouldDraw || IsBuffering) return;

            IRenderFrameBuffer sourceFbo = SourceFrameBufferByRef?.Invoke();
            if (sourceFbo == null || !sourceFbo.Active)
            {
                return;
            }

            if (ShaderProgram == null || !ShaderProgram.Active)
            {
                GameEngine.LogAppendLine(LogSeverity.Warning,
                    "OpenGL",
                    $"Tried to draw sprite ({Name}) without a shader program. Deactivating sprite component."
                );
                Active = false;
                return;
            }

            IRenderTargetMaterial sourceMaterial = sourceFbo.FrameBufferMaterial;

            if (MaintainAspectRatio && sourceMaterial.AspectRatio != CurrentAspectRatio)
            {
                CurrentAspectRatio = sourceMaterial.AspectRatio;
                BufferVertices();
            }

            ShaderProgram.Bind();
            ShaderProgram.SetVariable(OpenGlConstants.Uniforms.Model, WorldSpaceTransform.TransformMatrix);
            ShaderProgram.SetVariable(OpenGlConstants.Uniforms.View, viewMatrix);
            ShaderProgram.SetVariable(OpenGlConstants.Uniforms.Projection, projectionMatrix);

            GL.BindVertexArray(VertexArrayHandle);

            sourceMaterial.Bind();

            GL.DrawElements(
                BeginMode.Triangles,
                6,
                DrawElementsType.UnsignedShort,
                IntPtr.Zero
            );

            sourceMaterial.Unbind();

            GL.BindVertexArray(0);

            ShaderProgram.Unbind();
        }

        public override void DrawDebug(Matrix4 viewMatrix, Matrix4 projectionMatrix, IDebugDrawer debugDrawer)
        {
            if (!ShouldDraw || IsBuffering || !debugDrawer.DebugGeometryShaderProgram.Active) return;

            GameEngine.RenderEngine.WireFrameEnabled = true;

            debugDrawer.DebugGeometryShaderProgram.Bind();
            debugDrawer.DebugGeometryShaderProgram.SetVariable(OpenGlConstants.Uniforms.Model, WorldSpaceTransform.TransformMatrix);
            debugDrawer.DebugGeometryShaderProgram.SetVariable(OpenGlConstants.Uniforms.View, viewMatrix);
            debugDrawer.DebugGeometryShaderProgram.SetVariable(OpenGlConstants.Uniforms.Projection, projectionMatrix);
            debugDrawer.DebugGeometryShaderProgram.SetVariable(OpenGlConstants.Uniforms.Color, DebugColor);

            GL.BindVertexArray(VertexArrayHandle);

            GL.DrawElements(
                BeginMode.Triangles,
                6,
                DrawElementsType.UnsignedShort,
                IntPtr.Zero
            );

            GL.BindVertexArray(0);

            ShaderProgram.Unbind();

            GameEngine.RenderEngine.WireFrameEnabled = false;
        }

        public override void Destroy()
        {
            SetParent(null);
            base.Destroy();
        }

        protected override void CalculateVertices(out Vertex[] vertices, out short[] indices)
        {
            float width = CurrentAspectRatio.X / 2f;
            float height = CurrentAspectRatio.Y / 2f;

            vertices = new []
            {
                new Vertex(-width, height, 0f, 0f, 1f),
                new Vertex(-width, -height, 0f, 0f, 0f),
                new Vertex(width, -height, 0f, 1f, 0f),
                new Vertex(width, height, 0f, 1f, 1f)
            };
            indices = new short[]
            {
                0, 1, 2,
                2, 3, 0
            };
        }
    }
}