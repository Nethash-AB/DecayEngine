using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using DecayEngine.DecPakLib;
using DecayEngine.DecPakLib.DataStructure.Texture;
using DecayEngine.DecPakLib.Pointer;
using DecayEngine.DecPakLib.Resource;
using DecayEngine.DecPakLib.Resource.RootElement.Texture2D;
using DecayEngine.DecPakLib.Resource.RootElement.Texture3D;
using Pfim;
using TeximpNet.DDS;

namespace DecayEngine.ResourceBuilderLib.Resource.Compilers.RootElement.Texture
{
    public class TextureCompiler : IResourceCompiler<Texture2DResource>, IResourceCompiler<Texture3DResource>
    {
        public Stream Compile(IResource resource, ByReference<DataPointer> dataPointer, Stream sourceStream, out List<ByReference<DataPointer>> extraPointers)
        {
            extraPointers = new List<ByReference<DataPointer>>();

            return resource switch
            {
                Texture2DResource texture2DResource => Compile(texture2DResource, dataPointer, sourceStream, out extraPointers),
                Texture3DResource texture3DResource => Compile(texture3DResource, dataPointer, sourceStream, out extraPointers),
                _ => null
            };
        }

        public Stream Decompile(IResource resource, DataPointer dataPointer, Stream sourceStream, out List<DataPointer> extraPointers)
        {
            extraPointers = new List<DataPointer>();

            return resource switch
            {
                Texture2DResource texture2DResource => Decompile(texture2DResource, dataPointer, sourceStream, out extraPointers),
                Texture3DResource texture3DResource => Decompile(texture3DResource, dataPointer, sourceStream, out extraPointers),
                _ => null
            };
        }

        public Stream Compile(Texture2DResource resource, ByReference<DataPointer> dataPointer, Stream sourceStream, out List<ByReference<DataPointer>> extraPointers)
        {
            extraPointers = new List<ByReference<DataPointer>>();
            return CompileTexture(dataPointer, sourceStream);
        }

        public Stream Decompile(Texture2DResource resource, DataPointer dataPointer, Stream sourceStream, out List<DataPointer> extraPointers)
        {
            extraPointers = new List<DataPointer>();
            return DecompileTexture(dataPointer, sourceStream);
        }

        public Stream Compile(Texture3DResource resource, ByReference<DataPointer> dataPointer, Stream sourceStream, out List<ByReference<DataPointer>> extraPointers)
        {
            extraPointers = new List<ByReference<DataPointer>>();
            return CompileTexture(dataPointer, sourceStream);
        }

        public Stream Decompile(Texture3DResource resource, DataPointer dataPointer, Stream sourceStream, out List<DataPointer> extraPointers)
        {
            extraPointers = new List<DataPointer>();
            return DecompileTexture(dataPointer, sourceStream);
        }

        private static Stream CompileTexture(ByReference<DataPointer> dataPointer, Stream sourceStream)
        {
            string extension = Path.GetExtension(dataPointer().SourcePath);
            switch (extension)
            {
                case ".dectex":
                    return sourceStream;
                case ".dds":
                {
                    TextureDataStructure textureDataStructure = new TextureDataStructure();

                    DDSContainer ddsContainer = DDSFile.Read(sourceStream);
                    CompileCompressedDdsTexture(ddsContainer, out List<TextureMipMapDataStructure> compressedMipMaps, out TextureDataFormat compressedFormat);
                    textureDataStructure.CompressedFormat = compressedFormat;
                    textureDataStructure.CompressedMipMaps = compressedMipMaps;

                    sourceStream.Position = 0;
                    using (Dds image = Dds.Create(sourceStream, new PfimConfig()))
                    {
                        CompileUncompressedDdsTexture(image, out List<TextureMipMapDataStructure> uncompressedMipMaps, out TextureDataFormat uncompressedFormat);
                        textureDataStructure.UncompressedFormat = uncompressedFormat;
                        textureDataStructure.UncompressedMipMaps = uncompressedMipMaps;
                    }

                    return textureDataStructure.Serialize();
                }
                default:
                    return sourceStream;
            }
        }

        private static Stream DecompileTexture(DataPointer dataPointer, Stream sourceStream)
        {
            string extension = Path.GetExtension(dataPointer.SourcePath);
            switch (extension)
            {
                case ".dectex":
                    return sourceStream;
                case ".dds":
                {
                    TextureDataStructure textureDataStructure = new TextureDataStructure();
                    textureDataStructure.Deserialize(sourceStream);

                    return
                        DecompileCompressedDdsTexture(textureDataStructure.CompressedMipMaps, textureDataStructure.CompressedFormat);
                }
                default:
                    return sourceStream;
            }
        }

        private static void CompileCompressedDdsTexture(DDSContainer ddsContainer, out List<TextureMipMapDataStructure> mipMaps, out TextureDataFormat format)
        {
            mipMaps = new List<TextureMipMapDataStructure>();
            format = ddsContainer.Format switch
            {
                DXGIFormat.BC1_Typeless => TextureDataFormat.Dxt1,
                DXGIFormat.BC1_UNorm => TextureDataFormat.Dxt1,
                DXGIFormat.BC1_UNorm_SRGB => TextureDataFormat.Dxt1,
                DXGIFormat.BC2_Typeless => TextureDataFormat.Dxt3,
                DXGIFormat.BC2_UNorm => TextureDataFormat.Dxt3,
                DXGIFormat.BC2_UNorm_SRGB => TextureDataFormat.Dxt3,
                DXGIFormat.BC3_Typeless => TextureDataFormat.Dxt5,
                DXGIFormat.BC3_UNorm => TextureDataFormat.Dxt5,
                DXGIFormat.BC3_UNorm_SRGB => TextureDataFormat.Dxt5,
                _ => TextureDataFormat.Unknown
            };

            foreach (MipData mipData in ddsContainer.MipChains[0])
            {
                if (mipData.Width < 4 || mipData.Height < 4) break;

                byte[] mipDataBytes = new byte[mipData.SizeInBytes];
                Marshal.Copy(mipData.Data, mipDataBytes, 0, mipDataBytes.Length);

                TextureMipMapDataStructure mipMapDataStructure = new TextureMipMapDataStructure
                {
                    Width = mipData.Width,
                    Height = mipData.Height,
                    RowPitch = mipData.RowPitch,
                    PixelData = mipDataBytes
                };

                mipMaps.Add(mipMapDataStructure);
            }
        }

        private static void CompileUncompressedDdsTexture(Dds image, out List<TextureMipMapDataStructure> mipMaps, out TextureDataFormat format)
        {
            if (image.Compressed)
            {
                image.Decompress();
            }

            format = image.Format switch
            {
                ImageFormat.Rgba32 => TextureDataFormat.Rgba,
                ImageFormat.R5g5b5 => TextureDataFormat.Rgb5,
                ImageFormat.R5g5b5a1 => TextureDataFormat.Rgb5A1,
                ImageFormat.Rgb8 => TextureDataFormat.Rgb8,
                _ => TextureDataFormat.Unknown
            };

            mipMaps = new List<TextureMipMapDataStructure>
            {
                new TextureMipMapDataStructure
                {
                    Width = image.Width,
                    Height = image.Height,
                    RowPitch = (int) image.Header.PitchOrLinearSize,
                    PixelData = image.Data
                }
            };
        }

        private static Stream DecompileCompressedDdsTexture(
            IEnumerable<TextureMipMapDataStructure> mipMaps, TextureDataFormat format)
        {
            DDSContainer ddsContainer = new DDSContainer
            {
                Dimension = TextureDimension.Two,
                Format = format switch
                {
                    TextureDataFormat.Dxt1 => DXGIFormat.BC1_UNorm,
                    TextureDataFormat.Dxt3 => DXGIFormat.BC2_UNorm,
                    TextureDataFormat.Dxt5 => DXGIFormat.BC3_UNorm,
                    _ => DXGIFormat.Unknown
                },
                MipChains =
                {
                    new MipChain()
                }
            };

            List<GCHandle> handles = new List<GCHandle>();
            foreach (TextureMipMapDataStructure textureMipMapDataStructure in mipMaps)
            {
                GCHandle pixelDataHandle = GCHandle.Alloc(textureMipMapDataStructure.PixelData, GCHandleType.Pinned);
                handles.Add(pixelDataHandle);

                ddsContainer.MipChains[0].Add(
                    new MipData(
                        textureMipMapDataStructure.Width,
                        textureMipMapDataStructure.Height,
                        textureMipMapDataStructure.RowPitch,
                        pixelDataHandle.AddrOfPinnedObject(),
                        false
                    )
                );
            }

            MemoryStream ms = new MemoryStream();
            ddsContainer.Write(ms);

            handles.ForEach(h => h.Free());

            ms.Position = 0;
            return ms;
        }
    }
}