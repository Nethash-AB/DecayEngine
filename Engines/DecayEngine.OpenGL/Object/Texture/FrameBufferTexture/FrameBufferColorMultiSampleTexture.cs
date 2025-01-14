using DecayEngine.DecPakLib.Math.Vector;
using DecayEngine.ModuleSDK;
using DecayEngine.ModuleSDK.Engine.Render;
using DecayEngine.OpenGL.OpenGLInterop;

namespace DecayEngine.OpenGL.Object.Texture.FrameBufferTexture
{
    public class FrameBufferColorMultiSampleTexture : FrameBufferColorTexture
    {
        private readonly int _multiSampleCount;

        public FrameBufferColorMultiSampleTexture(int multiSampleCount)
        {
            _multiSampleCount = multiSampleCount;
        }

        public override void Bind()
        {
            GameEngine.RenderEngine.EngineThread.ExecuteOnThread(() =>
            {
                GL.ActiveTexture((int) TextureTargets.FrameBufferColor);
                GL.BindTexture(TextureTarget.Texture2DMultisample, TextureHandle);
                OpenGlGlobalState.TextureTargetState[TextureTargets.FrameBufferColor] = this;
            });
        }

        public override void BindAsRender()
        {
            GameEngine.RenderEngine.EngineThread.ExecuteOnThread(() =>
            {
                GL.ActiveTexture((int) TextureTargets.Color);
                GL.BindTexture(TextureTarget.Texture2DMultisample, TextureHandle);
                OpenGlGlobalState.TextureTargetState[TextureTargets.Color] = this;
            });
        }

        public override void Unbind()
        {
            GameEngine.RenderEngine.EngineThread.ExecuteOnThread(() =>
            {
                if (OpenGlGlobalState.TextureTargetState[TextureTargets.FrameBufferColor] == this)
                {
                    GL.ActiveTexture((int) TextureTargets.FrameBufferColor);
                    GL.BindTexture(TextureTarget.Texture2DMultisample, 0);
                }
                else if (OpenGlGlobalState.TextureTargetState[TextureTargets.Color] == this)
                {
                    GL.ActiveTexture((int) TextureTargets.Color);
                    GL.BindTexture(TextureTarget.Texture2DMultisample, 0);
                }
            });
        }

        public override void Load(Vector2 size, int attachmentPoint)
        {
            Width = (int) size.X;
            Height = (int) size.Y;

            GameEngine.RenderEngine.EngineThread.ExecuteOnThread(() =>
            {
                TextureHandle = GL.GenTexture();
                GL.BindTexture(TextureTarget.Texture2DMultisample, TextureHandle);

                PixelInternalFormat pixelInternalFormat =
                    GameEngine.RenderEngine.SupportsFeature(RenderEngineFeatures.Srgb)
                        ? PixelInternalFormat.SrgbAlpha
                        : PixelInternalFormat.Rgba;

                GL.TexImage2DMultisample(
                    TextureTargetMultisample.Texture2DMultisample, _multiSampleCount, pixelInternalFormat,
                    (int) size.X, (int) size.Y, true
                );

                GL.BindTexture(TextureTarget.Texture2DMultisample, 0);

                GL.FramebufferTexture2D(
                    FramebufferTarget.Framebuffer,
                    (FramebufferAttachment) attachmentPoint, TextureTarget.Texture2DMultisample,
                    TextureHandle, 0
                );
            });
        }
    }
}