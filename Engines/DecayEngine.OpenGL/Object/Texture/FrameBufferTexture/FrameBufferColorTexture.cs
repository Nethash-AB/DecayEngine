using System;
using DecayEngine.DecPakLib.Math.Vector;
using DecayEngine.ModuleSDK;
using DecayEngine.ModuleSDK.Engine.Render;
using DecayEngine.ModuleSDK.Object.Texture.RenderTargetTexture;
using DecayEngine.OpenGL.OpenGLInterop;

namespace DecayEngine.OpenGL.Object.Texture.FrameBufferTexture
{
    public class FrameBufferColorTexture : FrameBufferTexture, IRenderTargetColorTexture
    {
        public FrameBufferColorTexture(bool highPrecision = false) : base(highPrecision)
        {
        }

        public override void Bind()
        {
            GameEngine.RenderEngine.EngineThread.ExecuteOnThread(() =>
            {
                GL.ActiveTexture((int) TextureTargets.FrameBufferColor);
                GL.BindTexture(TextureTarget.Texture2D, TextureHandle);
                OpenGlGlobalState.TextureTargetState[TextureTargets.FrameBufferColor] = this;
            });
        }

        public override void BindAsRender()
        {
            GameEngine.RenderEngine.EngineThread.ExecuteOnThread(() =>
            {
                GL.ActiveTexture((int) TextureTargets.Color);
                GL.BindTexture(TextureTarget.Texture2D, TextureHandle);
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
                    GL.BindTexture(TextureTarget.Texture2D, 0);
                }
                else if (OpenGlGlobalState.TextureTargetState[TextureTargets.Color] == this)
                {
                    GL.ActiveTexture((int) TextureTargets.Color);
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

                PixelInternalFormat pixelInternalFormat;
                PixelType pixelType;
                if (GameEngine.RenderEngine.SupportsFeature(RenderEngineFeatures.Srgb))
                {
                    pixelInternalFormat = PixelInternalFormat.SrgbAlpha;
                    pixelType = PixelType.UnsignedByte;
                }
                else
                {
                    if (HighPrecission)
                    {
                        pixelInternalFormat = PixelInternalFormat.Rgba32f;
                        pixelType = PixelType.Float;
                    }
                    else
                    {
                        pixelInternalFormat = PixelInternalFormat.Rgba;
                        pixelType = PixelType.UnsignedByte;
                    }
                }

                GL.TexImage2D(
                    TextureTarget.Texture2D, 0, pixelInternalFormat,
                    (int) size.X, (int) size.Y, 0,
                    PixelFormat.Rgba, pixelType, IntPtr.Zero
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