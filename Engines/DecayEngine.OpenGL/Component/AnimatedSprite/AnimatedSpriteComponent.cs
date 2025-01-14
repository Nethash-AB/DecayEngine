using System;
using System.Linq;
using DecayEngine.DecPakLib;
using DecayEngine.DecPakLib.Math.Matrix;
using DecayEngine.DecPakLib.Math.Vector;
using DecayEngine.DecPakLib.Resource.RootElement.AnimatedMaterial;
using DecayEngine.DecPakLib.Resource.Structure.Math;
using DecayEngine.ModuleSDK;
using DecayEngine.ModuleSDK.Capability;
using DecayEngine.ModuleSDK.Component.Camera;
using DecayEngine.ModuleSDK.Component.Material;
using DecayEngine.ModuleSDK.Component.Sprite;
using DecayEngine.ModuleSDK.Exports.BaseExports.AnimatedSprite;
using DecayEngine.ModuleSDK.Logging;
using DecayEngine.ModuleSDK.Object.GameObject;
using DecayEngine.ModuleSDK.Object.Material;
using DecayEngine.ModuleSDK.Object.Scene;
using DecayEngine.OpenGL.Object.BufferStructure;
using DecayEngine.OpenGL.Object.Sprite;
using DecayEngine.OpenGL.OpenGLInterop;

namespace DecayEngine.OpenGL.Component.AnimatedSprite
{
    public class AnimatedSpriteComponent : Sprite, IAnimatedSprite
    {
        private IGameObject _parent;

        private IMaterial _material;
        private int _frame;
        private FrameData[] _frameData;

        public IGameObject Parent => _parent;
        public ByReference<IGameObject> ParentByRef => () => ref _parent;

        public Type ExportType => typeof(AnimatedSpriteExport);

        public override bool Active
        {
            get => Parent != null && base.Active;
            set => base.Active = value;
        }

        public IMaterial Material
        {
            get => _material;
            set
            {
                if (!(value is IAnimatedMaterial)) return;

                _material = value;
                Frame = 0;
            }
        }
        public ByReference<IMaterial> MaterialByRef => () => ref _material;

        public int Frame
        {
            get => _frame;
            set
            {
                if (Material == null)
                {
                    _frame = -1;
                }
                else
                {
                    if (value == _frame) return;

                    int maxFrame = ((IAnimatedMaterial) Material).AnimationFrames.Count - 1;
                    if (value < 0)
                    {
                        _frame = -1;
                    }
                    else if (value > maxFrame)
                    {
                        _frame = maxFrame;
                    }
                    else
                    {
                        _frame = value;
                    }

                    if (_frame < 0) return;

                    CalculateSpriteSize();
                }
            }
        }

        public AnimatedSpriteComponent()
        {
            _frame = -1;
        }

        ~AnimatedSpriteComponent()
        {
            Destroy();
        }

        public void SetParent(IGameObject parent)
        {
            _parent?.RemoveComponent(this);

            IAnimatedMaterial animatedMaterial = (IAnimatedMaterial) Material;
            if (animatedMaterial?.Parent == null)
            {
                animatedMaterial?.SetParent(parent);
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

        public override void Draw(Matrix4 viewMatrix, Matrix4 projectionMatrix)
        {
            if (!ShouldDraw || IsBuffering) return;

            if (ShaderProgram == null || !ShaderProgram.Active)
            {
                GameEngine.LogAppendLine(LogSeverity.Warning,
                    "OpenGL",
                    $"Tried to draw sprite ({Name}) without a shader program. Deactivating sprite component."
                );
                Active = false;
                return;
            }

            if (Material == null || !Material.Active)
            {
                GameEngine.LogAppendLine(LogSeverity.Warning,
                    "OpenGL",
                    $"Tried to draw sprite ({Name}) without a material. Deactivating sprite component."
                );
                Active = false;
                return;
            }

            if (MaintainAspectRatio && Material.AspectRatio != CurrentAspectRatio)
            {
                CurrentAspectRatio = Material.AspectRatio;
                BufferVertices();
            }

            ShaderProgram.Bind();
            ShaderProgram.SetVariable(OpenGlConstants.Uniforms.Model, WorldSpaceTransform.TransformMatrix);
            ShaderProgram.SetVariable(OpenGlConstants.Uniforms.View, viewMatrix);
            ShaderProgram.SetVariable(OpenGlConstants.Uniforms.Projection, projectionMatrix);

            GL.BindVertexArray(VertexArrayHandle);

            Material.Bind();

            GL.DrawElements(
                BeginMode.Triangles,
                _frameData[_frame].IndexCount,
                DrawElementsType.UnsignedShort,
                new IntPtr(_frameData[_frame].FirstIndexPointer * sizeof(short))
            );

            Material.Unbind();

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
                _frameData[_frame].IndexCount,
                DrawElementsType.UnsignedShort,
                new IntPtr(_frameData[_frame].FirstIndexPointer * sizeof(short))
            );

            GL.BindVertexArray(0);

            debugDrawer.DebugGeometryShaderProgram.Unbind();

            GameEngine.RenderEngine.WireFrameEnabled = false;
        }

        public override void Destroy()
        {
            SetParent(null);
            _material = null;
            base.Destroy();
        }

        private void CalculateSpriteSize()
        {
            DrawableSizeInternal = Vector3.Zero;

            if (!(Material is IAnimatedMaterial animatedMaterial)) return;

            AnimationFrameElement currentFrame = animatedMaterial.AnimationFrames.ElementAtOrDefault(Frame);
            if (currentFrame == null) return;

            float width = currentFrame.Vertices.Max(vertex => Math.Abs(vertex.X));
            float height = currentFrame.Vertices.Max(vertex => Math.Abs(vertex.Y));
            float depth = currentFrame.Vertices.Max(vertex => Math.Abs(vertex.Z));
            DrawableSizeInternal = new Vector3(width, height, depth);
        }

        protected override void CalculateVertices(out Vertex[] vertices, out short[] indices)
        {
            IAnimatedMaterial animatedMaterial = (IAnimatedMaterial) Material;

            vertices = animatedMaterial.AnimationFrames.SelectMany(f => f.Vertices).Select(v => new Vertex(v.X, v.Y, v.Z, v.U, v.V)).ToArray();
            indices = new short[animatedMaterial.AnimationFrames.Sum(f => f.Triangles.Count * 3)];

            _frameData = new FrameData[animatedMaterial.AnimationFrames.Count];

            int currentIndex = 0;
            int lastVertexIndex = 0;
            for (int i = 0; i < animatedMaterial.AnimationFrames.Count; i++)
            {
                AnimationFrameElement animationFrame = animatedMaterial.AnimationFrames[i];

                _frameData[i] = new FrameData
                {
                    FirstIndexPointer = currentIndex,
                    IndexCount = animationFrame.Triangles.Count * 3
                };

                int firstFrameVertex = 0;
                if (i > 0)
                {
                    lastVertexIndex += animatedMaterial.AnimationFrames[i - 1].Vertices.Count;
                    firstFrameVertex = lastVertexIndex;
                }

                foreach (TriangleStructure triangle in animationFrame.Triangles)
                {
                    indices[currentIndex] = (short) (triangle.Vertex1 + firstFrameVertex);
                    indices[currentIndex + 1] = (short) (triangle.Vertex2 + firstFrameVertex);
                    indices[currentIndex + 2] = (short) (triangle.Vertex3 + firstFrameVertex);
                    currentIndex += 3;
                }
            }
        }
    }
}