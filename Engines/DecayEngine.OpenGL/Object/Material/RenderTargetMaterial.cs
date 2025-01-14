using DecayEngine.DecPakLib.Math.Vector;
using DecayEngine.ModuleSDK;
using DecayEngine.ModuleSDK.Object.Material;
using DecayEngine.ModuleSDK.Object.Texture.RenderTargetTexture;
using DecayEngine.OpenGL.Object.Texture.FrameBufferTexture;
using DecayEngine.OpenGL.OpenGLInterop;

namespace DecayEngine.OpenGL.Object.Material
{
    public class RenderTargetMaterial : IRenderTargetMaterial
    {
        public bool Destroyed { get; private set; }
        public string Name { get; set; }

        public bool Active
        {
            get => ColorTexture != null && ColorTexture.Active || DepthStencilTexture != null && DepthStencilTexture.Active;
            set
            {
                if (!Active && value)
                {
                    if (ColorTexture != null)
                    {
                        ColorTexture.Active = true;
                    }

                    if (DepthStencilTexture != null)
                    {
                        DepthStencilTexture.Active = true;
                    }
                }
                else if (Active && !value)
                {
                    if (ColorTexture != null)
                    {
                        ColorTexture.Active = false;
                    }

                    if (DepthStencilTexture != null)
                    {
                        DepthStencilTexture.Active = false;
                    }
                }
            }
        }

        public Vector2 AspectRatio
        {
            get
            {
                float width = 1f;
                float height = 1f;

                if (ColorTexture != null)
                {
                    if (ColorTexture.Size.X > ColorTexture.Size.Y)
                    {
                        height = ColorTexture.Size.Y / ColorTexture.Size.X;
                    }
                    else if (ColorTexture.Size.X < ColorTexture.Size.Y)
                    {
                        width = ColorTexture.Size.X / ColorTexture.Size.Y;
                    }
                }
                else if (DepthStencilTexture != null)
                {
                    if (DepthStencilTexture.Size.X > DepthStencilTexture.Size.Y)
                    {
                        height = DepthStencilTexture.Size.Y / DepthStencilTexture.Size.X;
                    }
                    else if (DepthStencilTexture.Size.X < DepthStencilTexture.Size.Y)
                    {
                        width = DepthStencilTexture.Size.X / DepthStencilTexture.Size.Y;
                    }
                }

                return new Vector2(width, height);
            }
        }

        public IRenderTargetColorTexture ColorTexture { get; private set; }
        public IRenderTargetDepthStencilTexture DepthStencilTexture { get; private set; }

        public RenderTargetMaterial(int multiSampleCount = 0)
        {
            if (multiSampleCount > 0)
            {
                ColorTexture = new FrameBufferColorMultiSampleTexture(multiSampleCount);
                DepthStencilTexture = new FrameBufferDepthStencilMultiSampleTexture(multiSampleCount);
            }
            else
            {
                ColorTexture = new FrameBufferColorTexture();
                DepthStencilTexture = new FrameBufferDepthStencilTexture();
            }
        }

        ~RenderTargetMaterial()
        {
            Destroy();
        }

        public void Bind()
        {
            GameEngine.RenderEngine.EngineThread.ExecuteOnThread(() =>
            {
                if (ColorTexture != null && ColorTexture.Active)
                {
                    ColorTexture.BindAsRender();
                }

                if (DepthStencilTexture != null && DepthStencilTexture.Active)
                {
                    DepthStencilTexture.BindAsRender();
                }
            });
        }

        public void BindAsRenderTarget()
        {
            GameEngine.RenderEngine.EngineThread.ExecuteOnThread(() =>
            {
                if (ColorTexture != null && ColorTexture.Active)
                {
                    ColorTexture.Bind();
                }

                if (DepthStencilTexture != null && DepthStencilTexture.Active)
                {
                    DepthStencilTexture.Bind();
                }
            });
        }

        public void AttachColorComponents()
        {
            GameEngine.RenderEngine.EngineThread.ExecuteOnThread(() =>
            {
                if (ColorTexture != null && ColorTexture.Active)
                {
                    GL.DrawBuffers(1, new []{DrawBuffersEnum.ColorAttachment0});
                }
                else
                {
                    GL.DrawBuffers(1, new []{DrawBuffersEnum.None});
                }
            });
        }

        public void Unbind()
        {
            GameEngine.RenderEngine.EngineThread.ExecuteOnThread(() =>
            {
                if (ColorTexture != null && ColorTexture.Active)
                {
                    ColorTexture.Unbind();
                }

                if (DepthStencilTexture != null && DepthStencilTexture.Active)
                {
                    DepthStencilTexture.Unbind();
                }
            });
        }

        public void ReloadTextures(Vector2 size)
        {
            if (size == Vector2.Zero) return;

            if (ColorTexture.Active)
            {
                ColorTexture.Unload();
            }
            ColorTexture.Load(size, (int) FramebufferAttachment.ColorAttachment0);

            if (DepthStencilTexture.Active)
            {
                DepthStencilTexture.Unload();
            }
            DepthStencilTexture.Load(size, (int) FramebufferAttachment.DepthStencilAttachment);
        }

        public void Destroy()
        {
            ColorTexture.Active = false;
            ColorTexture.Destroy();
            ColorTexture = null;

            DepthStencilTexture.Active = false;
            DepthStencilTexture.Destroy();
            DepthStencilTexture = null;

            Destroyed = true;
        }
    }
}