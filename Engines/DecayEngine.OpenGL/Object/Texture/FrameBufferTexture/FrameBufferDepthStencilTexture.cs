using System;
using DecayEngine.DecPakLib.Math.Vector;
using DecayEngine.ModuleSDK;
using DecayEngine.ModuleSDK.Object.Texture.RenderTargetTexture;
using DecayEngine.OpenGL.OpenGLInterop;

namespace DecayEngine.OpenGL.Object.Texture.FrameBufferTexture
{
    public class FrameBufferDepthStencilTexture : FrameBufferTexture, IRenderTargetDepthStencilTexture
    {
        public override void Bind()
        {
            GameEngine.RenderEngine.EngineThread.ExecuteOnThread(() =>
            {
                GL.ActiveTexture((int) TextureTargets.FrameBufferDepthStencil);
                GL.BindTexture(TextureTarget.Texture2D, TextureHandle);
                OpenGlGlobalState.TextureTargetState[TextureTargets.FrameBufferDepthStencil] = this;
            });
        }

        public override void BindAsRender()
        {
            GameEngine.RenderEngine.EngineThread.ExecuteOnThread(() =>
            {
                GL.ActiveTexture((int) TextureTargets.Normal);
                GL.BindTexture(TextureTarget.Texture2D, TextureHandle);
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
                    GL.BindTexture(TextureTarget.Texture2D, 0);
                }
                else if (OpenGlGlobalState.TextureTargetState[TextureTargets.Normal] == this)
                {
                    GL.ActiveTexture((int) TextureTargets.Normal);
                    GL.BindTexture(TextureTarget.Texture2D, 0);
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
                GL.BindTexture(TextureTarget.Texture2D, TextureHandle);

                GL.TexImage2D(
                    TextureTarget.Texture2D, 0, PixelInternalFormat.Depth24Stencil8,
                    (int) size.X, (int) size.Y, 0,
                    PixelFormat.DepthStencil, PixelType.UnsignedInt248, IntPtr.Zero
                );

                GL.TexParameteri(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, TextureParameter.Nearest);
                GL.TexParameteri(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, TextureParameter.Nearest);

                GL.BindTexture(TextureTarget.Texture2D, 0);

                GL.FramebufferTexture2D(
                    FramebufferTarget.Framebuffer,
                    (FramebufferAttachment) attachmentPoint, TextureTarget.Texture2D,
                    TextureHandle, 0
                );
            });
        }
    }
}