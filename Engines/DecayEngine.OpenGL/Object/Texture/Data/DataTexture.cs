using System;
using DecayEngine.DecPakLib.Math.Vector;
using DecayEngine.ModuleSDK;
using DecayEngine.ModuleSDK.Object.Texture.Data;
using DecayEngine.OpenGL.OpenGLInterop;

namespace DecayEngine.OpenGL.Object.Texture.Data
{
    public class DataTexture : IDataTexture
    {
        private readonly TextureTargets _target;
        private readonly bool _highPrecision;
        private readonly uint _channelAmount;

        private uint _textureHandle;

        public bool Destroyed { get; private set; }
        public string Name { get; set; }

        public bool Active
        {
            get => _textureHandle > 0;
            set {}
        }

        public int Width { get; private set; }
        public int Height { get; private set; }
        public Vector2 Size => new Vector2(Width, Height);
        public int MipMapCount => 0;

        public DataTexture(TextureTargets target, uint channelAmount = 3, bool highPrecision = false)
        {
            _target = target;
            _highPrecision = highPrecision;

            if (channelAmount > 4)
            {
                channelAmount = 4;
            }
            else if (channelAmount == 0)
            {
                channelAmount = 1;
            }
            _channelAmount = channelAmount;
        }

        ~DataTexture()
        {
            Destroy();
        }

        public void Destroy()
        {
            Unload();

            _textureHandle = 0;

            Destroyed = true;
        }

        public void Bind()
        {
            GameEngine.RenderEngine.EngineThread.ExecuteOnThread(() =>
            {
                GL.ActiveTexture((int) _target);
                GL.BindTexture(TextureTarget.Texture2D, _textureHandle);
                OpenGlGlobalState.TextureTargetState[_target] = this;
            });
        }

        public void Unbind()
        {
            GameEngine.RenderEngine.EngineThread.ExecuteOnThread(() =>
            {
                if (OpenGlGlobalState.TextureTargetState[_target] != this) return;

                GL.ActiveTexture((int) _target);
                GL.BindTexture(TextureTarget.Texture2D, 0);
            });
        }

        public void Load(Vector2 size, int attachmentPoint)
        {
            Width = (int) size.X;
            Height = (int) size.Y;

            GameEngine.RenderEngine.EngineThread.ExecuteOnThread(() =>
            {
                _textureHandle = GL.GenTexture();
                GL.BindTexture(TextureTarget.Texture2D, _textureHandle);

                PixelInternalFormat pixelInternalFormat;
                PixelFormat pixelFormat;
                switch (_channelAmount)
                {
                    case 1:
                        pixelInternalFormat = _highPrecision ? PixelInternalFormat.R16f : PixelInternalFormat.R8;
                        pixelFormat = PixelFormat.Red;
                        break;
                    case 2:
                        pixelInternalFormat = _highPrecision ? PixelInternalFormat.Rg16f : PixelInternalFormat.Rg8;
                        pixelFormat = PixelFormat.Rg;
                        break;
                    case 3:
                        pixelInternalFormat = _highPrecision ? PixelInternalFormat.Rgb16f : PixelInternalFormat.Rgb8;
                        pixelFormat = PixelFormat.Rgb;
                        break;
                    case 4:
                        pixelInternalFormat = _highPrecision ? PixelInternalFormat.Rgba16f : PixelInternalFormat.Rgba8;
                        pixelFormat = PixelFormat.Rgba;
                        break;
                    default:
                        pixelInternalFormat = PixelInternalFormat.Rgba;
                        pixelFormat = PixelFormat.Rgba;
                        break;
                }
                PixelType pixelType = _highPrecision ? PixelType.Float : PixelType.UnsignedByte;

                GL.TexImage2D(
                    TextureTarget.Texture2D, 0, pixelInternalFormat,
                    (int) size.X, (int) size.Y, 0,
                    pixelFormat, pixelType, IntPtr.Zero
                );

                GL.TexParameteri(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, TextureParameter.Nearest);
                GL.TexParameteri(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, TextureParameter.Nearest);

                GL.BindTexture(TextureTarget.Texture2D, 0);

                GL.FramebufferTexture2D(
                    FramebufferTarget.Framebuffer,
                    (FramebufferAttachment) attachmentPoint, TextureTarget.Texture2D,
                    _textureHandle, 0
                );
            });
        }

        public void Unload()
        {
            GameEngine.RenderEngine.EngineThread.ExecuteOnThread(() =>
            {
                GL.DeleteTexture(_textureHandle);
                _textureHandle = 0;
            });
        }
    }
}