using System.Runtime.InteropServices;
using DecayEngine.DecPakLib.DataStructure.Texture;
using DecayEngine.DecPakLib.Math.Vector;
using DecayEngine.DecPakLib.Resource.RootElement.Texture3D;
using DecayEngine.ModuleSDK;
using DecayEngine.ModuleSDK.Capability;
using DecayEngine.ModuleSDK.Engine.Render;
using DecayEngine.ModuleSDK.Object.Texture.Texture3D;
using DecayEngine.OpenGL.OpenGLInterop;

namespace DecayEngine.OpenGL.Object.Texture.Texture3D
{
    public abstract class Texture3D : ITexture3D, IResourceable<Texture3DResource>
    {
        public bool Destroyed { get; private set; }
        public string Name { get; set; }

        public Texture3DResource Resource { get; set; }

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

        public int Width => _textureDataStructures[0].UncompressedMipMaps[0].Width;
        public int Height => _textureDataStructures[0].UncompressedMipMaps[0].Height;
        public Vector2 Size => new Vector2(Width, Height);

        protected uint TextureHandle;

        private TextureDataStructure[] _textureDataStructures;

        protected Texture3D(TextureDataStructure[] textureDataStructures)
        {
            _textureDataStructures = textureDataStructures;
        }

        ~Texture3D()
        {
            Destroy();
        }

        public void Destroy()
        {
            Unload();

            TextureHandle = 0;
            _textureDataStructures = null;

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
            GL.CreateTextures(TextureTarget.TextureCubeMap, 1, textureHandles);
            GL.BindTexture(TextureTarget.TextureCubeMap, textureHandles[0]);

            int width = Width;
            int height = Height;
            int mipMapCount = 0;
            while (width > 4 && height > 4)
            {
                mipMapCount++;
                width /= 4;
                height /= 4;
            }

            GL.TexParameteri(TextureTarget.TextureCubeMap, TextureParameterName.TextureBaseLevel, 0);
            GL.TexParameteri(TextureTarget.TextureCubeMap, TextureParameterName.TextureMaxLevel, mipMapCount - 1);
            GL.TexParameteri(TextureTarget.TextureCubeMap, TextureParameterName.TextureMagFilter, TextureParameter.Linear);
            GL.TexParameteri(TextureTarget.TextureCubeMap, TextureParameterName.TextureMinFilter, TextureParameter.LinearMipMapLinear);
            GL.TexParameteri(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapS, TextureParameter.ClampToEdge);
            GL.TexParameteri(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapT, TextureParameter.ClampToEdge);
            GL.TexParameteri(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapR, TextureParameter.ClampToEdge);

            PixelFormat pixelFormat;
            PixelInternalFormat pixelInternalFormat;
            int[] swizzleMask;
            switch (_textureDataStructures[0].UncompressedFormat)
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

            GL.TexParameteri(TextureTarget.TextureCubeMap, TextureParameterName.TextureSwizzleR, swizzleMask[0]);
            GL.TexParameteri(TextureTarget.TextureCubeMap, TextureParameterName.TextureSwizzleG, swizzleMask[1]);
            GL.TexParameteri(TextureTarget.TextureCubeMap, TextureParameterName.TextureSwizzleB, swizzleMask[2]);
            if (swizzleMask.Length > 3)
            {
                GL.TexParameteri(TextureTarget.TextureCubeMap, TextureParameterName.TextureSwizzleA, swizzleMask[3]);
            }

            GL.PixelStorei(PixelStoreParameter.UnpackAlignment, 1);
            GL.PixelStorei(PixelStoreParameter.UnpackRowLength, 0);
            GL.PixelStorei(PixelStoreParameter.UnpackSkipPixels, 0);
            GL.PixelStorei(PixelStoreParameter.UnpackSkipRows, 0);

            for (int i = 0; i < 6; i++)
            {
                TextureDataStructure textureDataStructure = _textureDataStructures[i];

                TextureTarget target = TextureTarget.TextureCubeMapPositiveX + i;

                GCHandle dataPtr = GCHandle.Alloc(textureDataStructure.UncompressedMipMaps[0].PixelData, GCHandleType.Pinned);
                GL.TexImage2D(target, 0, pixelInternalFormat,
                    Width, Height, 0,
                    pixelFormat, PixelType.UnsignedByte,
                    dataPtr.AddrOfPinnedObject()
                );
                dataPtr.Free();
            }

            GL.GenerateMipmap(GenerateMipmapTarget.TextureCubeMap);

            GL.BindTexture(TextureTarget.TextureCubeMap, 0);
            TextureHandle = textureHandles[0];
        }

        private void LoadCompressed()
        {
            uint[] textureHandles = new uint[1];
            GL.CreateTextures(TextureTarget.TextureCubeMap, 1, textureHandles);
            GL.BindTexture(TextureTarget.TextureCubeMap, textureHandles[0]);

            GL.TexParameteri(TextureTarget.TextureCubeMap, TextureParameterName.TextureBaseLevel, 0);
            GL.TexParameteri(TextureTarget.TextureCubeMap, TextureParameterName.TextureMaxLevel, _textureDataStructures[0].CompressedMipMaps.Count - 1);
            GL.TexParameteri(TextureTarget.TextureCubeMap, TextureParameterName.TextureMagFilter, TextureParameter.Linear);
            GL.TexParameteri(TextureTarget.TextureCubeMap, TextureParameterName.TextureMinFilter, TextureParameter.LinearMipMapLinear);
            GL.TexParameteri(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapS, TextureParameter.ClampToEdge);
            GL.TexParameteri(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapT, TextureParameter.ClampToEdge);
            GL.TexParameteri(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapR, TextureParameter.ClampToEdge);

            PixelInternalFormat pixelInternalFormat = _textureDataStructures[0].CompressedFormat switch
            {
                TextureDataFormat.Dxt1 => PixelInternalFormat.CompressedRgbaS3tcDxt1Ext,
                TextureDataFormat.Dxt3 => PixelInternalFormat.CompressedRgbaS3tcDxt3Ext,
                TextureDataFormat.Dxt5 => PixelInternalFormat.CompressedRgbaS3tcDxt5Ext,
                _ => PixelInternalFormat.CompressedRgbaS3tcDxt3Ext
            };

            for (int i = 0; i < 6; i++)
            {
                TextureDataStructure textureDataStructure = _textureDataStructures[i];

                TextureTarget target = TextureTarget.TextureCubeMapPositiveX + i;

                for (int j = 0; j < textureDataStructure.CompressedMipMaps.Count; j++)
                {
                    if (textureDataStructure.CompressedMipMaps[j].Width <= 4 || textureDataStructure.CompressedMipMaps[j].Height <= 4)
                    {
                        GL.TexParameteri(TextureTarget.TextureCubeMap, TextureParameterName.TextureMaxLevel, j - 1);
                        break;
                    }

                    GCHandle dataPtr = GCHandle.Alloc(textureDataStructure.CompressedMipMaps[j].PixelData, GCHandleType.Pinned);
                    GL.CompressedTexImage2D(target, j, pixelInternalFormat,
                        textureDataStructure.CompressedMipMaps[j].Width, textureDataStructure.CompressedMipMaps[j].Height, 0,
                        textureDataStructure.CompressedMipMaps[j].PixelData.Length, dataPtr.AddrOfPinnedObject()
                    );
                    dataPtr.Free();
                }
            }

            GL.BindTexture(TextureTarget.TextureCubeMap, 0);
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