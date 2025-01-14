using DecayEngine.DecPakLib.Math.Vector;
using DecayEngine.ModuleSDK;
using DecayEngine.ModuleSDK.Object.Texture.RenderTargetTexture;
using DecayEngine.OpenGL.OpenGLInterop;

namespace DecayEngine.OpenGL.Object.Texture.FrameBufferTexture
{
    public abstract class FrameBufferTexture : IRenderTargetTexture
    {
        protected uint TextureHandle;
        protected bool HighPrecission;

        public bool Destroyed { get; private set; }
        public string Name { get; set; }

        public bool Active
        {
            get => TextureHandle > 0;
            set {}
        }

        public int Width { get; protected set; }
        public int Height { get; protected set; }
        public Vector2 Size => new Vector2(Width, Height);
        public int MipMapCount => 0;

        protected FrameBufferTexture(bool highPrecision = false)
        {
            HighPrecission = highPrecision;
        }

        ~FrameBufferTexture()
        {
            Destroy();
        }

        public void Destroy()
        {
            Unload();

            TextureHandle = 0;

            Destroyed = true;
        }

        public abstract void Bind();
        public abstract void BindAsRender();

        public abstract void Unbind();

        public abstract void Load(Vector2 size, int attachmentPoint);

        public void Unload()
        {
            GameEngine.RenderEngine.EngineThread.ExecuteOnThread(() =>
            {
                GL.DeleteTexture(TextureHandle);
                TextureHandle = 0;
            });
        }
    }
}