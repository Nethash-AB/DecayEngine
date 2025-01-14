using System;
using DecayEngine.DecPakLib;
using DecayEngine.DecPakLib.Math.Matrix;
using DecayEngine.DecPakLib.Math.Vector;
using DecayEngine.ModuleSDK;
using DecayEngine.ModuleSDK.Capability;
using DecayEngine.ModuleSDK.Component.Camera;
using DecayEngine.ModuleSDK.Component.ShaderProgram;
using DecayEngine.ModuleSDK.Component.Sprite;
using DecayEngine.ModuleSDK.Exports.BaseExports.TextSprite;
using DecayEngine.ModuleSDK.Logging;
using DecayEngine.ModuleSDK.Object.GameObject;
using DecayEngine.ModuleSDK.Object.Scene;
using DecayEngine.OpenGL.Object.Text;
using DecayEngine.OpenGL.Object.Texture;
using DecayEngine.OpenGL.OpenGLInterop;

namespace DecayEngine.OpenGL.Component.TextSprite
{
    public class TextSpriteComponent : TextDrawer, ITextSprite
    {
        private IGameObject _parent;

        private int _shouldDrawFlag;
        private readonly Vector4 _debugColor;

        public string Name { get; set; }

        public IGameObject Parent => _parent;
        public ByReference<IGameObject> ParentByRef => () => ref _parent;

        public Type ExportType => typeof(TextSpriteExport);

        public override bool Active
        {
            get
            {
                if (Parent == null)
                {
                    return false;
                }

                return VertexArrayHandle > 0;
            }
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

        public ByReference<IShaderProgram> ShaderProgramByRef => () => ref ShaderProgramInternal;

        public override bool ShouldDraw
        {
            get => _shouldDrawFlag != 0;
            set => _shouldDrawFlag = value ? 1 : 0;
        }

        public TextSpriteComponent()
        {
            _shouldDrawFlag = -1;

            Random rand = new Random();
            Vector3 color = new Vector3((float) rand.NextDouble(), (float) rand.NextDouble(), (float) rand.NextDouble()).Normalized;
            _debugColor = new Vector4(color, 1f);
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

        ~TextSpriteComponent()
        {
            Destroy();
        }

        public override void Draw(Matrix4 viewMatrix, Matrix4 projectionMatrix)
        {
            if (!ShouldDraw || IsBuffering) return;

            if (!ShaderProgram.Active)
            {
                GameEngine.LogAppendLine(LogSeverity.Warning,
                    "OpenGL",
                    $"Tried to draw text sprite ({Name}) without a shader program. Deactivating text sprite component."
                );
                Active = false;
                return;
            }

            ShaderProgram.Bind();
            ShaderProgram.SetVariable(OpenGlConstants.Uniforms.Model, WorldSpaceTransform.TransformMatrix);
            ShaderProgram.SetVariable(OpenGlConstants.Uniforms.View, viewMatrix);
            ShaderProgram.SetVariable(OpenGlConstants.Uniforms.Projection, projectionMatrix);
            ShaderProgram.SetVariable(OpenGlConstants.Uniforms.Color, Color);

            GL.BindVertexArray(VertexArrayHandle);

            GL.ActiveTexture((int) TextureTargets.Color);
            GL.BindTexture(TextureTarget.Texture2D, TextureHandle);

            for (int i = 0; i < GlyphCount * 2; i++)
            {
                GL.DrawArrays(BeginMode.Triangles, i * 3, 3);
            }

            GL.BindTexture(TextureTarget.Texture2D, 0);

            GL.BindVertexArray(0);

            ShaderProgram.Unbind();
        }

        public void DrawDebug(Matrix4 viewMatrix, Matrix4 projectionMatrix, IDebugDrawer debugDrawer)
        {
            if (!ShouldDraw || IsBuffering || !debugDrawer.DebugGeometryShaderProgram.Active) return;

            GameEngine.RenderEngine.WireFrameEnabled = true;

            Matrix4 worldTransformMatrix = WorldSpaceTransform.TransformMatrix;

            debugDrawer.DebugGeometryShaderProgram.Bind();
            debugDrawer.DebugGeometryShaderProgram.SetVariable(OpenGlConstants.Uniforms.Model, worldTransformMatrix);
            debugDrawer.DebugGeometryShaderProgram.SetVariable(OpenGlConstants.Uniforms.View, viewMatrix);
            debugDrawer.DebugGeometryShaderProgram.SetVariable(OpenGlConstants.Uniforms.Projection, projectionMatrix);
            debugDrawer.DebugGeometryShaderProgram.SetVariable(OpenGlConstants.Uniforms.Color, _debugColor);

            GL.BindVertexArray(VertexArrayHandle);

            for (int i = 0; i < GlyphCount * 2; i++)
            {
                GL.DrawArrays(BeginMode.Triangles, i * 3, 3);
            }

            GL.BindVertexArray(0);

            ShaderProgram.Unbind();

            GameEngine.RenderEngine.WireFrameEnabled = false;

            // 2 from ViewSpaceBBox providing halves and 2 from the box vertices being half distance from the center
            debugDrawer.AddWireframeSquare(
                WorldSpaceTransform.Position - Pivot / 4f,
                WorldSpaceTransform.Rotation,
                WorldSpaceTransform.Scale * DrawableSizeInternal / 4f,
                Vector4.One - _debugColor
            );
        }
    }
}