using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using DecayEngine.DecPakLib;
using DecayEngine.DecPakLib.Pointer;
using DecayEngine.DecPakLib.Resource;
using DecayEngine.DecPakLib.Resource.RootElement.Font;
using DecayEngine.DecPakLib.Resource.Structure.Component.Font;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.Primitives;
using SixLabors.Shapes;
using Glyph = DecayEngine.DecPakLib.Resource.Structure.Component.Font.Glyph;
using Path = System.IO.Path;
using PointF = SixLabors.Primitives.PointF;

namespace DecayEngine.ResourceBuilderLib.Resource.Compilers.RootElement.Font
{
    public class FontCompiler : IResourceCompiler<FontResource>
    {
        public Stream Compile(IResource resource, ByReference<DataPointer> dataPointer, Stream sourceStream, out List<ByReference<DataPointer>> extraPointers)
        {
            extraPointers = new List<ByReference<DataPointer>>();
            if (!(resource is FontResource specificResource)) return null;
            return Compile(specificResource, dataPointer, sourceStream, out extraPointers);
        }

        public Stream Decompile(IResource resource, DataPointer dataPointer, Stream sourceStream, out List<DataPointer> extraPointers)
        {
            extraPointers = new List<DataPointer>();
            if (!(resource is FontResource specificResource)) return null;
            return Decompile(specificResource, dataPointer, sourceStream, out extraPointers);
        }

        private static List<(string, SixLabors.Fonts.Glyph, IPathCollection)> GenerateGlyphVectors(SixLabors.Fonts.Font font, IEnumerable<string> validCharacters)
        {
            List<(string, SixLabors.Fonts.Glyph, IPathCollection)> glyphVectors = new List<(string, SixLabors.Fonts.Glyph, IPathCollection)>();
            foreach (string character in validCharacters)
            {
                try
                {
                    IPathCollection glyphPaths = TextBuilder.GenerateGlyphs(character, new RendererOptions(font, 72f));
                    SixLabors.Fonts.Glyph glyph = font.GetGlyph(character[0]);
                    glyphVectors.Add((character, glyph, glyphPaths));
                }
                catch
                {
                    // ignored
                }
            }

            return glyphVectors;
        }

        private static FontMipMap GenerateFontMipMap(int mipMapSize)
        {
            string tempFile = Path.GetTempFileName();
            return new FontMipMap
            {
                Texture = new DataPointer
                {
                    SourcePath = $"./mipmaps/{mipMapSize}.png",
                    FullSourcePath = tempFile
                },
                Size = mipMapSize
            };
        }

        public Stream Compile(FontResource resource, ByReference<DataPointer> dataPointer, Stream sourceStream, out List<ByReference<DataPointer>> extraPointers)
        {
            extraPointers = new List<ByReference<DataPointer>>();

            FontCollection fontCollection = new FontCollection();
            fontCollection.Install(sourceStream, out FontDescription fontDescription);
            SixLabors.Fonts.Font font = fontCollection.CreateFont(fontDescription.FontFamily, 100f);

            List<string> validCharacters = new List<string>{"\0"};
            for (int i = 0; i < 128; i++)
            {
                string charString = $"{(char) i}";
                if (!char.IsControl((char) i) && !char.IsWhiteSpace((char) i) && !validCharacters.Contains(charString))
                {
                    validCharacters.Add(charString);
                }
            }

            List<(string, SixLabors.Fonts.Glyph, IPathCollection)> glyphs = GenerateGlyphVectors(font, validCharacters);
            float maxGlyphHeight = glyphs.Max(tuple => tuple.Item3.Bounds.Height);

            int gridSideLength = (int) Math.Ceiling(Math.Sqrt(validCharacters.Count));
            RectangleF[,] grid = new RectangleF[gridSideLength, gridSideLength];
            for (int j = 0; j < gridSideLength; j++)
            {
                for (int i = 0; i < gridSideLength; i++)
                {
                    grid[i, j] = new RectangleF(i + 0.1f, j + 0.1f, 0.9f, 0.9f);
                }
            }

            int mipMapSize = 256 * gridSideLength;
            while (mipMapSize % 4 > 0 && Math.Sqrt(mipMapSize) % 4 > 0)
            {
                mipMapSize++;
            }

            while (mipMapSize > 96 || mipMapSize % 4 != 0)
            {
                FontMipMap mipMap = GenerateFontMipMap(mipMapSize);
                using (Image<Rgba32> glyphImage = new Image<Rgba32>(mipMapSize, mipMapSize))
                {
                    float gridCellSize = (float) mipMapSize / gridSideLength;
                    glyphImage.Mutate(context =>
                    {
                        Size imageSize = context.GetCurrentSize();

                        context.Fill(new Rgba32(0, 0, 0, 0));

                        Stack<(string, SixLabors.Fonts.Glyph, IPathCollection)> glyphStack =
                            new Stack<(string, SixLabors.Fonts.Glyph, IPathCollection)>(glyphs.OrderByDescending(tuple => tuple.Item1));
                        for (int i = 0; i < grid.GetLength(1); i++)
                        for (int j = 0; j < grid.GetLength(0); j++)
                        {
                            RectangleF unscaledGridCell = grid[j, i];
                            if (glyphStack.Count < 1) break;
                            (string character, SixLabors.Fonts.Glyph glyph, IPathCollection glyphPaths) = glyphStack.Pop();

                            RectangleF gridCell = unscaledGridCell;
                            gridCell.X *= gridCellSize;
                            gridCell.Y *= gridCellSize;
                            gridCell.Width *= gridCellSize;
                            gridCell.Height *= gridCellSize;

                            float reductionFactor = 1000f / unscaledGridCell.Height;
                            float glyphScalingRatio = imageSize.Height / reductionFactor;
                            Vector2 glyphPosition = new Vector2(
                                gridCell.X,
                                gridCell.Y
                            );

                            Matrix3x2 transformationMatrix = Matrix3x2.CreateScale(glyphScalingRatio) * Matrix3x2.CreateTranslation(glyphPosition);
                            glyphPaths = glyphPaths.Transform(transformationMatrix);

                            float outLineThickness = 0f;
                            if (resource.OutlineOnly)
                            {
                                outLineThickness = resource.OutlineThickness * imageSize.Height / reductionFactor;
                                context.Draw(
                                    new GraphicsOptions(true),
                                    Rgba32.White,
                                    outLineThickness,
                                    glyphPaths
                                );
                            }
                            else
                            {
                                context.Fill(
                                    new GraphicsOptions(true),
                                    Rgba32.White,
                                    glyphPaths
                                );
                            }

                            float glyphHeight = gridCell.Top + Math.Min(gridCell.Height, maxGlyphHeight * glyphScalingRatio) + outLineThickness / 2f;

                            float glyphTop = Math.Min(gridCell.Top, glyphPaths.Bounds.Top) - outLineThickness / 2f;
                            float glyphAscendShift = glyphPaths.Bounds.Top < gridCell.Top ? gridCell.Top - glyphPaths.Bounds.Top : 0f;

                            Glyph glyphObj = new Glyph
                            {
                                Character = character,
                                UMin = (glyphPaths.Bounds.Left - outLineThickness / 2f) / imageSize.Width,
                                VMin = (glyphTop) / imageSize.Height,
                                UMax = (glyphPaths.Bounds.Right + outLineThickness / 2f) / imageSize.Width,
                                VMax = (glyphHeight - glyphAscendShift) / imageSize.Height,
                                KerningTable = new List<KerningTableEntry>()
                            };

                            foreach (string nextChar in validCharacters)
                            {
                                SixLabors.Fonts.Glyph rightGlyph = font.GetGlyph(nextChar[0]);
                                float kerning = font.Instance.GetOffset(glyph.Instance, rightGlyph.Instance).X;
                                if (glyphObj.KerningTable.Any(kt => kt.Character == nextChar)) continue;

                                float value = kerning / glyph.Instance.SizeOfEm * gridCellSize / imageSize.Width;
                                glyphObj.KerningTable.Add(new KerningTableEntry(nextChar, value));
                            }

                            if (resource.Glyphs.All(g => g.Character != character))
                            {
                                resource.Glyphs.Add(glyphObj);
                            }
                        }
                    });

                    Console.WriteLine($"Rendered Font MipMap of size: {mipMapSize}.");

                    using FileStream fs = File.OpenWrite(mipMap.Texture.FullSourcePath);
                    foreach (Rgba32 pixel in glyphImage.GetPixelSpan())
                    {
                        fs.WriteByte(pixel.A);
                    }
                }

                resource.MipMaps.Add(mipMap);
                extraPointers.Add(mipMap.TextureByReference);

                mipMapSize /= 2;
            }

            return sourceStream;
        }

        public Stream Decompile(FontResource resource, DataPointer dataPointer, Stream sourceStream, out List<DataPointer> extraPointers)
        {
            extraPointers = new List<DataPointer>();

            foreach (FontMipMap mipMap in resource.MipMaps)
            {
                Rgba32[] pixelData;
                using (MemoryStream ms = mipMap.Texture.GetData())
                {
                    pixelData = new Rgba32[ms.Length];
                    for (int i = 0; i < pixelData.Length; i++)
                    {
                        byte luminance = (byte) ms.ReadByte();

                        pixelData[i] = new Rgba32(255, 255, 255, luminance);
                    }
                }

                string tempFile = Path.GetTempFileName();
                DataPointer imagePointer = new DataPointer
                {
                    SourcePath = Path.Combine(Path.GetDirectoryName(resource.MetaFilePath), mipMap.Texture.SourcePath),
                    FullSourcePath = tempFile
                };

                using (Image<Rgba32> glyphImage = Image.LoadPixelData(pixelData, mipMap.Size, mipMap.Size))
                {
                    using FileStream fs = File.OpenWrite(tempFile);
                    glyphImage.Mutate(context =>
                    {
                        float thickNess = Math.Max(mipMap.Size / 250, 1);
//                        float thickNess = 1f;
                        foreach (Glyph glyph in resource.Glyphs)
                        {
                            PointF[] points =
                            {
                                new PointF(glyph.UMin * mipMap.Size, glyph.VMin * mipMap.Size),
                                new PointF(glyph.UMax * mipMap.Size, glyph.VMin * mipMap.Size),
                                new PointF(glyph.UMax * mipMap.Size, glyph.VMax * mipMap.Size),
                                new PointF(glyph.UMin * mipMap.Size, glyph.VMax * mipMap.Size),
                                new PointF(glyph.UMin * mipMap.Size, glyph.VMin * mipMap.Size)
                            };
                            context.DrawPolygon(Rgba32.Black, thickNess, points);
                        }
                    });
                    glyphImage.SaveAsPng(fs);
                }

                extraPointers.Add(imagePointer);
            }

            return sourceStream;
        }
    }
}