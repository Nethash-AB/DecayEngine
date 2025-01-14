using System.Runtime.InteropServices;
using DecayEngine.DecPakLib.DataStructure.Texture;
using DecayEngine.DecPakLib.Math.Vector;
using DecayEngine.DecPakLib.Resource.RootElement.Texture2D;
using DecayEngine.ModuleSDK;
using DecayEngine.ModuleSDK.Capability;
using DecayEngine.ModuleSDK.Engine.Render;
using DecayEngine.ModuleSDK.Object.Texture.Texture2D;
using DecayEngine.OpenGL.OpenGLInterop;

namespace DecayEngine.OpenGL.Object.Texture.Texture2D
{
    public abstract class Texture2D : ITexture2D, IResourceable<Texture2DResource>
    {
        public bool Destroyed { get; private set; }
        public string Name { get; set; }

        public Texture2DResource Resource { get; set; }

        public bool Active
        {
            get => TextureHandle > 0;
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

        public int Width => _textureDataStructure.UncompressedMipMaps[0].Width;
        public int Height => _textureDataStructure.UncompressedMipMaps[0].Height;
        public Vector2 Size => new Vector2(Width, Height);

        protected uint TextureHandle;

        private TextureDataStructure _textureDataStructure;

        protected Texture2D(TextureDataStructure textureDataStructure)
        {
            _textureDataStructure = textureDataStructure;
        }

        ~Texture2D()
        {
            Destroy();
        }

        public void Destroy()
        {
            Unload();

            TextureHandle = 0;
            _textureDataStructure = null;

            Destroyed = true;
        }

        private void Load()
        {
            GameEngine.RenderEngine.EngineThread.ExecuteOnThread(() =>
            {
                if (GameEngine.RenderEngine.SupportsFeature(RenderEngineFeatures.CompressedTextures))
                {
                    LoadCompressed();
                }
                else
                {
                    LoadUncompressed();
                }
            });
        }

        private void LoadUncompressed()
        {
            uint[] textureHandles = new uint[1];
            GL.CreateTextures(TextureTarget.Texture2D, 1, textureHandles);
            GL.BindTexture(TextureTarget.Texture2D, textureHandles[0]);

            int width = Width;
            int height = Height;
            int mipMapCount = 0;
            while (width > 4 && height > 4)
            {
                mipMapCount++;
                width /= 4;
                height /= 4;
            }

            GL.TexParameteri(TextureTarget.Texture2D, TextureParameterName.TextureBaseLevel, 0);
            GL.TexParameteri(TextureTarget.Texture2D, TextureParameterName.TextureMaxLevel, mipMapCount - 1);
            GL.TexParameteri(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, TextureParameter.Linear);
            GL.TexParameteri(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, TextureParameter.LinearMipMapLinear);

            PixelFormat pixelFormat;
            PixelInternalFormat pixelInternalFormat;
            int[] swizzleMask;
            switch (_textureDataStructure.UncompressedFormat)
            {
                case TextureDataFormat.Rgb5:
                    pixelFormat = PixelFormat.Rgb;
                    pixelInternalFormat = PixelInternalFormat.Rgb5;
                    swizzleMask = new [] {(int) TextureParameter.Blue, (int) TextureParameter.Green, (int) TextureParameter.Red};
                    break;
                case TextureDataFormat.Rgb8:
                    pixelFormat = PixelFormat.Rgb;
                    pixelInternalFormat = PixelInternalFormat.Rgb8;
                    swizzleMask = new [] {(int) TextureParameter.Blue, (int) TextureParameter.Green, (int) TextureParameter.Red};
                    break;
                case TextureDataFormat.Rgb5A1:
                    pixelFormat = PixelFormat.Rgb;
                    pixelInternalFormat = PixelInternalFormat.Rgb5A1;
                    swizzleMask = new [] {(int) TextureParameter.Blue, (int) TextureParameter.Green, (int) TextureParameter.Red, (int) TextureParameter.Alpha};
                    break;
                default:
                    pixelFormat = PixelFormat.Rgba;
                    pixelInternalFormat = PixelInternalFormat.Rgba;
                    swizzleMask = new [] {(int) TextureParameter.Blue, (int) TextureParameter.Green, (int) TextureParameter.Red, (int) TextureParameter.Alpha};
                    break;
            }

            GL.TexParameteri(TextureTarget.Texture2D, TextureParameterName.TextureSwizzleR, swizzleMask[0]);
            GL.TexParameteri(TextureTarget.Texture2D, TextureParameterName.TextureSwizzleG, swizzleMask[1]);
            GL.TexParameteri(TextureTarget.Texture2D, TextureParameterName.TextureSwizzleB, swizzleMask[2]);
            if (swizzleMask.Length > 3)
            {
                GL.TexParameteri(TextureTarget.Texture2D, TextureParameterName.TextureSwizzleA, swizzleMask[3]);
            }

            GL.PixelStorei(PixelStoreParameter.UnpackAlignment, 1);
            GL.PixelStorei(PixelStoreParameter.UnpackRowLength, 0);
            GL.PixelStorei(PixelStoreParameter.UnpackSkipPixels, 0);
            GL.PixelStorei(PixelStoreParameter.UnpackSkipRows, 0);

            GCHandle dataPtr = GCHandle.Alloc(_textureDataStructure.UncompressedMipMaps[0].PixelData, GCHandleType.Pinned);
            GL.TexImage2D(TextureTarget.Texture2D, 0, pixelInternalFormat,
                Width, Height, 0,
                pixelFormat, PixelType.UnsignedByte,
                dataPtr.AddrOfPinnedObject()
            );
            dataPtr.Free();

            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

            GL.BindTexture(TextureTarget.Texture2D, 0);
            TextureHandle = textureHandles[0];
        }

        private void LoadCompressed()
        {
            uint[] textureHandles = new uint[1];
            GL.CreateTextures(TextureTarget.Texture2D, 1, textureHandles);
            GL.BindTexture(TextureTarget.Texture2D, textureHandles[0]);

            GL.TexParameteri(TextureTarget.Texture2D, TextureParameterName.TextureBaseLevel, 0);
            GL.TexParameteri(TextureTarget.Texture2D, TextureParameterName.TextureMaxLevel, _textureDataStructure.CompressedMipMaps.Count - 1);
            GL.TexParameteri(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, TextureParameter.Linear);
            GL.TexParameteri(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, TextureParameter.LinearMipMapLinear);

            PixelInternalFormat pixelInternalFormat = _textureDataStructure.CompressedFormat switch
            {
                TextureDataFormat.Dxt1 => PixelInternalFormat.CompressedRgbaS3tcDxt1Ext,
                TextureDataFormat.Dxt3 => PixelInternalFormat.CompressedRgbaS3tcDxt3Ext,
                TextureDataFormat.Dxt5 => PixelInternalFormat.CompressedRgbaS3tcDxt5Ext,
                _ => PixelInternalFormat.CompressedRgbaS3tcDxt3Ext
            };

            for (int i = 0; i < _textureDataStructure.CompressedMipMaps.Count; i++)
            {
                if (_textureDataStructure.CompressedMipMaps[i].Width <= 4 || _textureDataStructure.CompressedMipMaps[i].Height <= 4)
                {
                    GL.TexParameteri(TextureTarget.Texture2D, TextureParameterName.TextureMaxLevel, i - 1);
                    break;
                }

                GCHandle dataPtr = GCHandle.Alloc(_textureDataStructure.CompressedMipMaps[i].PixelData, GCHandleType.Pinned);
                GL.CompressedTexImage2D(TextureTarget.Texture2D, i, pixelInternalFormat,
                    _textureDataStructure.CompressedMipMaps[i].Width, _textureDataStructure.CompressedMipMaps[i].Height, 0,
                    _textureDataStructure.CompressedMipMaps[i].PixelData.Length, dataPtr.AddrOfPinnedObject()
                );
                dataPtr.Free();
            }

            GL.BindTexture(TextureTarget.Texture2D, 0);
            TextureHandle = textureHandles[0];
        }

        private void Unload()
        {
            GameEngine.RenderEngine.EngineThread.ExecuteOnThread(() =>
            {
                GL.DeleteTexture(TextureHandle);
                TextureHandle = 0;
            });
        }

        public abstract void Bind();
        public abstract void Unbind();
    }
}