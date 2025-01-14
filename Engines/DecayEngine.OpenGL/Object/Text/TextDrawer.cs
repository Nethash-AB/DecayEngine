using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using DecayEngine.DecPakLib;
using DecayEngine.DecPakLib.Math;
using DecayEngine.DecPakLib.Math.Matrix;
using DecayEngine.DecPakLib.Math.Vector;
using DecayEngine.DecPakLib.Resource.RootElement.Font;
using DecayEngine.DecPakLib.Resource.Structure.Component.Font;
using DecayEngine.ModuleSDK;
using DecayEngine.ModuleSDK.Capability;
using DecayEngine.ModuleSDK.Component.ShaderProgram;
using DecayEngine.ModuleSDK.Component.Sprite;
using DecayEngine.ModuleSDK.Object.TextDrawer;
using DecayEngine.ModuleSDK.Object.Transform;
using DecayEngine.OpenGL.Object.BufferStructure;
using DecayEngine.OpenGL.Object.Texture;
using DecayEngine.OpenGL.OpenGLInterop;

namespace DecayEngine.OpenGL.Object.Text
{
    public class TextDrawer : ITextDrawer
    {
        private Transform _transform;
        private string _text;
        private float _size;
        private float _characterSeparation;
        private float _whiteSpaceSeparation;
        private TextAlignmentHorizontal _alignmentHorizontal;
        private TextAlignmentVertical _alignmentVertical;

        private readonly List<(float lineWidth, List<Tuple<float, Vertex[]>> lineVertices)> _glyphVertexMap;
        private float _maxLineWidth;
        private float _lineHeight;

        protected uint VertexBufferHandle;
        protected uint VertexArrayHandle;
        protected bool IsBuffering;
        protected int GlyphCount;
        protected uint TextureHandle;

        protected Vector3 PivotInternal;
        protected Vector3 DrawableSizeInternal;
        protected IShaderProgram ShaderProgramInternal;

        public bool Destroyed { get; private set; }

        public FontResource Resource { get; set; }
        public virtual bool Active
        {
            get => VertexArrayHandle > 0;
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

        public Transform Transform => _transform;
        public ByReference<Transform> TransformByRef => () => ref _transform;
        public Transform WorldSpaceTransform => this.GetWorldSpaceTransform();
        public bool IsPbrCapable => false;

        public IShaderProgram ShaderProgram
        {
            get => ShaderProgramInternal;
            set => ShaderProgramInternal = value;
        }

        public Vector3 Pivot => PivotInternal;
        public Vector3 DrawableSize => DrawableSizeInternal * Transform.Scale;

        public bool AutoUpdateOnChange { get; set; }
        public Vector4 Color { get; set; }
        public string Text
        {
            get => _text;
            set
            {
                if (value == _text) return;

                _text = value;

                if (!AutoUpdateOnChange) return;
                Update();
            }
        }
        public float FontSize
        {
            get => _size * 10f;
            set
            {
                float scaledSize = value / 10f;
                if (scaledSize.IsApproximately(_size)) return;

                _size = scaledSize;

                if (!AutoUpdateOnChange) return;
                Update();
            }
        }

        public float CharacterSeparation
        {
            get => _characterSeparation * 250f;
            set
            {
                float scaledValue = value / 250f;
                if (scaledValue.IsApproximately(_characterSeparation)) return;

                _characterSeparation = scaledValue;

                if (!AutoUpdateOnChange) return;
                Update();
            }
        }

        public float WhiteSpaceSeparation
        {
            get => _whiteSpaceSeparation * 25f;
            set
            {
                float scaledValue = value / 25f;
                if (scaledValue.IsApproximately(_whiteSpaceSeparation)) return;

                _whiteSpaceSeparation = scaledValue;

                if (!AutoUpdateOnChange) return;
                Update();
            }
        }

        public TextAlignmentHorizontal AlignmentHorizontal
        {
            get => _alignmentHorizontal;
            set
            {
                if (value == _alignmentHorizontal) return;

                _alignmentHorizontal = value;

                if (!AutoUpdateOnChange) return;
                Update();
            }
        }

        public TextAlignmentVertical AlignmentVertical
        {
            get => _alignmentVertical;
            set
            {
                if (value == _alignmentVertical) return;

                _alignmentVertical = value;

                if (!AutoUpdateOnChange) return;
                Update();
            }
        }

        public virtual bool ShouldDraw
        {
            get => true;
            set {}
        }

        public TextDrawer()
        {
            _transform = new Transform();
            _text = "";

            FontSize = 1f;
            CharacterSeparation = 1f;
            WhiteSpaceSeparation = 1f;

            _glyphVertexMap = new List<(float lineWidth, List<Tuple<float, Vertex[]>> lineVertices)>();

            AutoUpdateOnChange = true;
        }

        ~TextDrawer()
        {
            Destroy();
        }

        public virtual void Draw(Matrix4 viewMatrix, Matrix4 projectionMatrix)
        {
            if (IsBuffering || !ShaderProgram.Active) return;

            ShaderProgram.Bind();
            ShaderProgram.SetVariable(OpenGlConstants.Uniforms.Model, WorldSpaceTransform.TransformMatrix);
            ShaderProgram.SetVariable(OpenGlConstants.Uniforms.View, viewMatrix);
            ShaderProgram.SetVariable(OpenGlConstants.Uniforms.Projection, projectionMatrix);
            ShaderProgram.SetVariable(OpenGlConstants.Uniforms.Color, Color);

            GL.BindVertexArray(VertexArrayHandle);

            GL.ActiveTexture((int) TextureTargets.Color);
            GL.BindTexture(TextureTarget.Texture2D, TextureHandle);

            for (int i = 0; i < GlyphCount * 2; i++)
            {
                GL.DrawArrays(BeginMode.Triangles, i * 3, 3);
            }

            GL.BindTexture(TextureTarget.Texture2D, 0);

            GL.BindVertexArray(0);

            ShaderProgram.Unbind();
        }

        public void Update()
        {
            GenerateGlyphMap();

            if (!Active) return;
            BufferVertices();
        }

        public virtual void Destroy()
        {
            Unload();

            ShaderProgram = null;
            _transform = null;

            Destroyed = true;
        }

        protected void Load()
        {
            GameEngine.RenderEngine.EngineThread.ExecuteOnThread(() =>
            {
                List<FontMipMap> mipMaps = Resource.MipMaps;

                GL.PixelStorei(PixelStoreParameter.UnpackAlignment, 1);

                uint[] textureHandles = new uint[1];
                GL.CreateTextures(TextureTarget.Texture2D, 1, textureHandles);
                GL.BindTexture(TextureTarget.Texture2D, textureHandles[0]);

                GL.TexParameteri(TextureTarget.Texture2D, TextureParameterName.TextureBaseLevel, 0);
                GL.TexParameteri(TextureTarget.Texture2D, TextureParameterName.TextureMaxLevel, mipMaps.Count - 1);
                GL.TexParameteri(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, TextureParameter.Linear);
                GL.TexParameteri(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, TextureParameter.LinearMipMapLinear);
                GL.TexParameteri(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, TextureParameter.ClampToEdge);
                GL.TexParameteri(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, TextureParameter.ClampToEdge);

                for (int level = 0; level < mipMaps.Count; level++)
                {
                    FontMipMap mipMap = mipMaps[level];

                    byte[] mipMapData = mipMap.Texture.GetDataAsByteArray();
                    GCHandle dataHandle = GCHandle.Alloc(mipMapData, GCHandleType.Pinned);
                    GL.TexImage2D(TextureTarget.Texture2D, level, PixelInternalFormat.R8,
                        mipMap.Size, mipMap.Size, 0,
                        PixelFormat.Red, PixelType.UnsignedByte, dataHandle.AddrOfPinnedObject()
                    );
                    dataHandle.Free();
                }

                GL.BindTexture(TextureTarget.Texture2D, 0);

                TextureHandle = textureHandles[0];

                VertexArrayHandle = GL.GenVertexArray();
                VertexBufferHandle = GL.GenBuffer();

                BufferVertices(true);

                if (GameEngine.RenderEngine.IsEmbedded)
                {
                    // Position
                    GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, Vertex.Size, IntPtr.Zero);
                    GL.EnableVertexAttribArray(0);

                    // UV
                    GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, Vertex.Size, new IntPtr(3 * sizeof(float)));
                    GL.EnableVertexAttribArray(1);
                }
                else
                {
                    // Position
                    GL.VertexAttribBinding(0, 0);
                    GL.EnableVertexAttribArray(0);
                    GL.VertexAttribFormat(0, 3, VertexAttribFormat.Float, false, 0);

                    // UV
                    GL.VertexAttribBinding(1, 0);
                    GL.EnableVertexAttribArray(1);
                    GL.VertexAttribFormat(1, 2, VertexAttribFormat.Float, false, 3 * sizeof(float));

                    GL.BindVertexBuffer(0, VertexBufferHandle, IntPtr.Zero, new IntPtr(Vertex.Size));
                }

                GL.BindVertexArray(0);
                GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            });
        }

        protected void Unload()
        {
            GameEngine.RenderEngine.EngineThread.ExecuteOnThread(() =>
            {
                GL.DeleteVertexArray(VertexArrayHandle);
                VertexArrayHandle = 0;
                GL.DeleteBuffer(VertexBufferHandle);
                VertexBufferHandle = 0;

                GL.DeleteTexture(TextureHandle);
                TextureHandle = 0;

                IsBuffering = false;
            });
        }

        protected void GenerateGlyphMap()
        {
            _glyphVertexMap.Clear();

            string[] lines = _text.Split('\n');

            foreach (string line in lines)
            {
                List<Tuple<float, Vertex[]>> lineMap = new List<Tuple<float, Vertex[]>>();
                float lineWidth = 0f;
                _lineHeight = 0f;
                string[] characters = Regex.Split(line, @"([\p{C}].|.)").Where(s => s != string.Empty).ToArray();
                for (int i = 0; i < characters.Length; i++)
                {
                    string character = characters[i];

                    string nextCharacter = null;
                    Glyph nextGlyph = null;
                    if (i + 1 < characters.Length)
                    {
                        nextCharacter = Resource.Glyphs.Any(g => g.Character == characters[i + 1]) ? characters[i + 1] : "\0";
                        nextGlyph = Resource.Glyphs.First(g => g.Character == nextCharacter);
                    }

                    Glyph glyph;

                    Vertex[] vertices;
                    if (string.IsNullOrWhiteSpace(character))
                    {
                        vertices = null;
                        glyph = null;
                        character = " ";
                    }
                    else
                    {
                        glyph = Resource.Glyphs.FirstOrDefault(g => g.Character == character) ?? Resource.Glyphs.First(g => g.Character == "\0");
                        vertices = GetVerticesForGlyph(glyph);
                    }

                    float kerning = 0f;

                    if (glyph != null)
                    {
                        if (nextCharacter != null)
                        {
                            kerning = -(
                                glyph.KerningTable.First(kt => kt.Character == nextCharacter).Kerning +
                                nextGlyph.KerningTable.First(kt => kt.Character == character).Kerning
                            );
                        }

                        if (i + 1 < characters.Length) // Don't apply character padding to the last character
                        {
                            lineWidth += GetGlyphWidth(glyph) - kerning + _characterSeparation / _size;
                        }
                        else
                        {
                            lineWidth += GetGlyphWidth(glyph) - kerning;
                        }

                        float glyphHeight = GetGlyphHeight(glyph);
                        if (glyphHeight > _lineHeight)
                        {
                            _lineHeight = glyphHeight;
                        }
                    }
                    else
                    {
                        if (i + 1 < characters.Length) // Trim whitespace padding from the end
                        {
                            lineWidth += _whiteSpaceSeparation;
                        }
                    }

                    lineMap.Add(new Tuple<float, Vertex[]>(kerning, vertices));
                }

                _glyphVertexMap.Add((lineWidth, lineMap));
            }

            _maxLineWidth = 0f;
            foreach ((float lineWidth, List<Tuple<float, Vertex[]>> lineVertices) in _glyphVertexMap)
            {
                if (lineVertices.Count < 1) continue;

                if (lineWidth > _maxLineWidth)
                {
                    _maxLineWidth = lineWidth;
                }
            }

            DrawableSizeInternal = new Vector3(_maxLineWidth, _lineHeight * _glyphVertexMap.Count, 0f) * 2f;

            PivotInternal = Vector3.Zero;
            PivotInternal.X = _alignmentHorizontal switch
            {
                TextAlignmentHorizontal.Left => -DrawableSizeInternal.X,
                TextAlignmentHorizontal.Center => 0f,
                TextAlignmentHorizontal.Right => DrawableSizeInternal.X,
                _ => PivotInternal.X
            };
            PivotInternal.Y = _alignmentVertical switch
            {
                TextAlignmentVertical.Top => DrawableSizeInternal.Y,
                TextAlignmentVertical.Center => 0f,
                TextAlignmentVertical.Bottom => -DrawableSizeInternal.Y,
                _ => PivotInternal.Y
            };
        }

        protected static float GetGlyphWidth(Glyph glyph)
        {
            return (glyph.UMax - glyph.UMin);
        }

        protected static float GetGlyphHeight(Glyph glyph)
        {
            return (glyph.VMax - glyph.VMin);
        }

        protected static Vertex[] GetVerticesForGlyph(Glyph glyph)
        {
            float vertexWidth = GetGlyphWidth(glyph);
            float vertexHeight = GetGlyphHeight(glyph);

            return new[]
            {
                // Triangle 1
                new Vertex(0, 0f, 0f, glyph.UMin, glyph.VMin),
                new Vertex(0, -vertexHeight, 0f, glyph.UMin, glyph.VMax),
                new Vertex(vertexWidth, -vertexHeight, 0f, glyph.UMax, glyph.VMax),

                // Triangle 2
                new Vertex(0, 0f, 0f, glyph.UMin, glyph.VMin),
                new Vertex(vertexWidth, -vertexHeight, 0f, glyph.UMax, glyph.VMax),
                new Vertex(vertexWidth, 0f, 0f, glyph.UMax, glyph.VMin),
            };
        }

        protected void BufferVertices(bool leaveOpen = false)
        {
            if (IsBuffering) return;

            IsBuffering = true;
            GameEngine.RenderEngine.EngineThread.ExecuteOnThread(() =>
            {
                GL.BindVertexArray(VertexArrayHandle);

                GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBufferHandle);

                if (!string.IsNullOrEmpty(_text) && _text.Length > 0)
                {
                    List<Vertex> scaleVertices = new List<Vertex>();

                    float linePosition = AlignmentVertical switch
                    {
                        TextAlignmentVertical.Center => (_lineHeight * _glyphVertexMap.Count / 2f),
                        TextAlignmentVertical.Top => 0f,
                        _ => (_lineHeight * _glyphVertexMap.Count)
                    };

                    foreach ((float lineWidth, List<Tuple<float, Vertex[]>> lineVertices) in _glyphVertexMap)
                    {
                        float charPosition = AlignmentHorizontal switch
                        {
                            TextAlignmentHorizontal.Center => -(lineWidth / 2f),
                            TextAlignmentHorizontal.Left => 0f,
                            _ => lineWidth
                        };

                        foreach ((float kerning, Vertex[] charVertices) in lineVertices)
                        {
                            if (charVertices == null)
                            {
                                if (AlignmentHorizontal == TextAlignmentHorizontal.Right)
                                {
                                    charPosition -= _whiteSpaceSeparation + kerning;
                                }
                                else
                                {
                                    charPosition += _whiteSpaceSeparation - kerning;
                                }

                                continue;
                            }

                            foreach (Vertex vertex in charVertices)
                            {
                                Vertex vert = vertex;
                                if (AlignmentHorizontal == TextAlignmentHorizontal.Right)
                                {
                                    vert.Position = new Vector3(
                                        (vert.Position.X - charPosition) * _size,
                                        (vert.Position.Y + linePosition) * _size,
                                        0
                                    );
                                }
                                else
                                {
                                    vert.Position = new Vector3(
                                        (vert.Position.X + charPosition) * _size,
                                        (vert.Position.Y + linePosition) * _size,
                                        0
                                    );
                                }

                                scaleVertices.Add(vert);
                            }

                            if (AlignmentHorizontal == TextAlignmentHorizontal.Right)
                            {
                                charPosition -= charVertices[2].Position.X - kerning + _characterSeparation / _size;
                            }
                            else
                            {
                                charPosition += charVertices[2].Position.X - kerning + _characterSeparation / _size;
                            }
                        }

                        linePosition -= _lineHeight;
                    }

                    GlyphCount = scaleVertices.Count / 6;

                    Vertex[] vertices = scaleVertices.ToArray();
                    GCHandle vertexHandle = GCHandle.Alloc(vertices, GCHandleType.Pinned);

                    GL.BufferData(BufferTarget.ArrayBuffer,
                        new IntPtr(Vertex.Size * vertices.Length),
                        vertexHandle.AddrOfPinnedObject(),
                        BufferUsageHint.DynamicDraw
                    );

                    vertexHandle.Free();
                }
                else
                {
                    GL.BufferData(BufferTarget.ArrayBuffer, new IntPtr(Vertex.Size), IntPtr.Zero, BufferUsageHint.DynamicDraw);
                    DrawableSizeInternal = Vector3.Zero;
                }

                if (leaveOpen)
                {
                    IsBuffering = false;
                    return;
                }

                GL.BindVertexArray(0);
                GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

                IsBuffering = false;
            });
        }
    }
}