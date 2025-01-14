using DecayEngine.DecPakLib.Math.Vector;
using DecayEngine.ModuleSDK;
using DecayEngine.ModuleSDK.Object.Material;
using DecayEngine.ModuleSDK.Object.Texture.Data;
using DecayEngine.ModuleSDK.Object.Texture.RenderTargetTexture;
using DecayEngine.ModuleSDK.Object.Texture.Texture3D;
using DecayEngine.OpenGL.Object.Texture;
using DecayEngine.OpenGL.Object.Texture.Data;
using DecayEngine.OpenGL.Object.Texture.FrameBufferTexture;
using DecayEngine.OpenGL.OpenGLInterop;

namespace DecayEngine.OpenGL.Object.Material
{
    public class DeferredShadingMaterial : IDeferredShadingMaterial
    {
        public bool Destroyed { get; private set; }
        public string Name { get; set; }

        public bool Active
        {
            get => ColorTexture != null && ColorTexture.Active ||
                   DepthStencilTexture != null && DepthStencilTexture.Active ||
                   PositionTexture != null && PositionTexture.Active ||
                   NormalsTexture != null && NormalsTexture.Active ||
                   MetallicityRoughnessTexture != null && MetallicityRoughnessTexture.Active ||
                   EmissionTexture != null && EmissionTexture.Active ||
                   EnvironmentTexture != null && EnvironmentTexture.Active;
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

                    if (PositionTexture != null)
                    {
                        PositionTexture.Active = true;
                    }

                    if (NormalsTexture != null)
                    {
                        NormalsTexture.Active = true;
                    }

                    if (MetallicityRoughnessTexture != null)
                    {
                        MetallicityRoughnessTexture.Active = true;
                    }

                    if (EmissionTexture != null)
                    {
                        EmissionTexture.Active = true;
                    }

                    if (EnvironmentTexture != null)
                    {
                        EnvironmentTexture.Active = true;
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

                    if (PositionTexture != null)
                    {
                        PositionTexture.Active = false;
                    }

                    if (NormalsTexture != null)
                    {
                        NormalsTexture.Active = false;
                    }

                    if (MetallicityRoughnessTexture != null)
                    {
                        MetallicityRoughnessTexture.Active = false;
                    }

                    if (EmissionTexture != null)
                    {
                        EmissionTexture.Active = false;
                    }

                    if (EnvironmentTexture != null)
                    {
                        EnvironmentTexture.Active = false;
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
                else if (PositionTexture != null)
                {
                    if (PositionTexture.Size.X > PositionTexture.Size.Y)
                    {
                        height = PositionTexture.Size.Y / PositionTexture.Size.X;
                    }
                    else if (PositionTexture.Size.X < PositionTexture.Size.Y)
                    {
                        width = PositionTexture.Size.X / PositionTexture.Size.Y;
                    }
                }
                else if (NormalsTexture != null)
                {
                    if (NormalsTexture.Size.X > NormalsTexture.Size.Y)
                    {
                        height = NormalsTexture.Size.Y / NormalsTexture.Size.X;
                    }
                    else if (NormalsTexture.Size.X < NormalsTexture.Size.Y)
                    {
                        width = NormalsTexture.Size.X / NormalsTexture.Size.Y;
                    }
                }
                else if (MetallicityRoughnessTexture != null)
                {
                    if (MetallicityRoughnessTexture.Size.X > MetallicityRoughnessTexture.Size.Y)
                    {
                        height = MetallicityRoughnessTexture.Size.Y / MetallicityRoughnessTexture.Size.X;
                    }
                    else if (MetallicityRoughnessTexture.Size.X < MetallicityRoughnessTexture.Size.Y)
                    {
                        width = MetallicityRoughnessTexture.Size.X / MetallicityRoughnessTexture.Size.Y;
                    }
                }
                else if (EmissionTexture != null)
                {
                    if (EmissionTexture.Size.X > EmissionTexture.Size.Y)
                    {
                        height = EmissionTexture.Size.Y / EmissionTexture.Size.X;
                    }
                    else if (EmissionTexture.Size.X < EmissionTexture.Size.Y)
                    {
                        width = EmissionTexture.Size.X / EmissionTexture.Size.Y;
                    }
                }
                else if (EnvironmentTexture != null)
                {
                    if (EnvironmentTexture.Size.X > EnvironmentTexture.Size.Y)
                    {
                        height = EnvironmentTexture.Size.Y / EnvironmentTexture.Size.X;
                    }
                    else if (EnvironmentTexture.Size.X < EnvironmentTexture.Size.Y)
                    {
                        width = EnvironmentTexture.Size.X / EnvironmentTexture.Size.Y;
                    }
                }

                return new Vector2(width, height);
            }
        }

        public IRenderTargetColorTexture ColorTexture { get; private set; }
        public IRenderTargetDepthStencilTexture DepthStencilTexture { get; private set; }
        public IDataTexture PositionTexture { get; private set; }
        public IDataTexture NormalsTexture { get; private set; }
        public IDataTexture MetallicityRoughnessTexture { get; private set; }
        public IDataTexture EmissionTexture { get; private set; }
        public IEnvironmentTexture EnvironmentTexture { get; set; }

        public DeferredShadingMaterial()
        {
            ColorTexture = new FrameBufferColorTexture(true);
            DepthStencilTexture = new FrameBufferDepthStencilTexture();
            PositionTexture = new DataTexture(TextureTargets.FrameBufferPosition, 3, true);
            NormalsTexture = new DataTexture(TextureTargets.FrameBufferNormals, 3, true);
            MetallicityRoughnessTexture = new DataTexture(TextureTargets.FrameBufferMetallicityRoughness, 2, true);
            EmissionTexture = new DataTexture(TextureTargets.FrameBufferEmission, 4);
        }

        ~DeferredShadingMaterial()
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

                if (PositionTexture != null && PositionTexture.Active)
                {
                    PositionTexture.Bind();
                }

                if (NormalsTexture != null && NormalsTexture.Active)
                {
                    NormalsTexture.Bind();
                }

                if (MetallicityRoughnessTexture != null && MetallicityRoughnessTexture.Active)
                {
                    MetallicityRoughnessTexture.Bind();
                }

                if (EmissionTexture != null && EmissionTexture.Active)
                {
                    EmissionTexture.Bind();
                }

                if (EnvironmentTexture != null && EnvironmentTexture.Active)
                {
                    EnvironmentTexture.Bind();
                }
            });
        }

        public void AttachColorComponents()
        {
            GameEngine.RenderEngine.EngineThread.ExecuteOnThread(() =>
            {
                DrawBuffersEnum[] drawBuffers = new DrawBuffersEnum[5];

                if (ColorTexture != null && ColorTexture.Active)
                {
                    drawBuffers[0] = DrawBuffersEnum.ColorAttachment0;
                }
                else
                {
                    drawBuffers[0] = DrawBuffersEnum.None;
                }

                if (PositionTexture != null && PositionTexture.Active)
                {
                    drawBuffers[1] = DrawBuffersEnum.ColorAttachment1;
                }
                else
                {
                    drawBuffers[1] = DrawBuffersEnum.None;
                }

                if (NormalsTexture != null && NormalsTexture.Active)
                {
                    drawBuffers[2] = DrawBuffersEnum.ColorAttachment2;
                }
                else
                {
                    drawBuffers[2] = DrawBuffersEnum.None;
                }

                if (MetallicityRoughnessTexture != null && MetallicityRoughnessTexture.Active)
                {
                    drawBuffers[3] = DrawBuffersEnum.ColorAttachment3;
                }
                else
                {
                    drawBuffers[3] = DrawBuffersEnum.None;
                }

                if (EmissionTexture != null && EmissionTexture.Active)
                {
                    drawBuffers[4] = DrawBuffersEnum.ColorAttachment4;
                }
                else
                {
                    drawBuffers[4] = DrawBuffersEnum.None;
                }

                GL.DrawBuffers(drawBuffers.Length, drawBuffers);
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

                if (PositionTexture != null && PositionTexture.Active)
                {
                    PositionTexture.Unbind();
                }

                if (NormalsTexture != null && NormalsTexture.Active)
                {
                    NormalsTexture.Unbind();
                }

                if (MetallicityRoughnessTexture != null && MetallicityRoughnessTexture.Active)
                {
                    MetallicityRoughnessTexture.Unbind();
                }

                if (EmissionTexture != null && EmissionTexture.Active)
                {
                    EmissionTexture.Unbind();
                }

                if (EnvironmentTexture != null && EnvironmentTexture.Active)
                {
                    EnvironmentTexture.Unbind();
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

            if (PositionTexture.Active)
            {
                PositionTexture.Unload();
            }
            PositionTexture.Load(size, (int) FramebufferAttachment.ColorAttachment1);

            if (NormalsTexture.Active)
            {
                NormalsTexture.Unload();
            }
            NormalsTexture.Load(size, (int) FramebufferAttachment.ColorAttachment2);

            if (MetallicityRoughnessTexture.Active)
            {
                MetallicityRoughnessTexture.Unload();
            }
            MetallicityRoughnessTexture.Load(size, (int) FramebufferAttachment.ColorAttachment3);

            if (EmissionTexture.Active)
            {
                EmissionTexture.Unload();
            }
            EmissionTexture.Load(size, (int) FramebufferAttachment.ColorAttachment4);

            if (EnvironmentTexture != null && !EnvironmentTexture.Active)
            {
                EnvironmentTexture.Active = true;
            }
        }

        public void Destroy()
        {
            ColorTexture.Active = false;
            ColorTexture.Destroy();
            ColorTexture = null;

            DepthStencilTexture.Active = false;
            DepthStencilTexture.Destroy();
            DepthStencilTexture = null;

            PositionTexture.Active = false;
            PositionTexture.Destroy();
            PositionTexture = null;

            NormalsTexture.Active = false;
            NormalsTexture.Destroy();
            NormalsTexture = null;

            MetallicityRoughnessTexture.Active = false;
            MetallicityRoughnessTexture.Destroy();
            MetallicityRoughnessTexture = null;

            EmissionTexture.Active = false;
            EmissionTexture.Destroy();
            EmissionTexture = null;

            EnvironmentTexture.Active = false;
            EnvironmentTexture.Destroy();
            EnvironmentTexture = null;

            Destroyed = true;
        }
    }
}