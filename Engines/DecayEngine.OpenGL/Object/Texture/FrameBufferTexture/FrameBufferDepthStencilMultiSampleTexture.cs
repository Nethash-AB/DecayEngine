using DecayEngine.DecPakLib.Math.Vector;
using DecayEngine.ModuleSDK;
using DecayEngine.OpenGL.OpenGLInterop;

namespace DecayEngine.OpenGL.Object.Texture.FrameBufferTexture
{
    public class FrameBufferDepthStencilMultiSampleTexture : FrameBufferDepthStencilTexture
    {
        private readonly int _multiSampleCount;

        public FrameBufferDepthStencilMultiSampleTexture(int multiSampleCount)
        {
            _multiSampleCount = multiSampleCount;
        }

        public override void Bind()
        {
            GameEngine.RenderEngine.EngineThread.ExecuteOnThread(() =>
            {
                GL.ActiveTexture((int) TextureTargets.FrameBufferDepthStencil);
                GL.BindTexture(TextureTarget.Texture2DMultisample, TextureHandle);
                OpenGlGlobalState.TextureTargetState[TextureTargets.FrameBufferDepthStencil] = this;
            });
        }

        public override void BindAsRender()
        {
            GameEngine.RenderEngine.EngineThread.ExecuteOnThread(() =>
            {
                GL.ActiveTexture((int) TextureTargets.Normal);
                GL.BindTexture(TextureTarget.Texture2DMultisample, TextureHandle);
                OpenGlGlobalState.TextureTargetState[TextureTargets.Normal] = this;
            });
        }

        public override void Unbind()
        {
            GameEngine.RenderEngine.EngineThread.ExecuteOnThread(() =>
            {
                if (OpenGlGlobalState.TextureTargetState[TextureTargets.FrameBufferDepthStencil] == this)
                {
                    GL.ActiveTexture((int) TextureTargets.FrameBufferDepthStencil);
                    GL.BindTexture(TextureTarget.Texture2DMultisample, 0);
                }
                else if (OpenGlGlobalState.TextureTargetState[TextureTargets.Normal] == this)
                {
                    GL.ActiveTexture((int) TextureTargets.Normal);
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

                GL.TexImage2DMultisample(
                    TextureTargetMultisample.Texture2DMultisample, _multiSampleCount, PixelInternalFormat.Depth24Stencil8,
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