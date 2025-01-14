using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using DecayEngine.DecPakLib.Math.Matrix;
using DecayEngine.DecPakLib.Math.Vector;
using DecayEngine.ModuleSDK;
using DecayEngine.ModuleSDK.Component.Camera;
using DecayEngine.ModuleSDK.Component.ShaderProgram;
using DecayEngine.ModuleSDK.Object.TextDrawer;
using DecayEngine.ModuleSDK.Object.Transform;
using DecayEngine.OpenGL.Object.BufferStructure;
using DecayEngine.OpenGL.OpenGLInterop;

namespace DecayEngine.OpenGL.Debug
{
    public class OpenGlDebugDrawer : IDebugDrawer
    {
        public bool Destroyed { get; private set; }

        public bool Active
        {
            get => _vertexArrayHandle > 0;
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

        public IShaderProgram DebugGeometryShaderProgram { get; set; }
        public IShaderProgram DebugLinesShaderProgram { get; set; }
        public IShaderProgram DebugTextShaderProgram { get; set; }
        public ITextDrawer DebugTextDrawer { get; set; }

        private uint _vertexBufferHandle;
        private uint _vertexArrayHandle;
        private bool _isBuffering;

        private readonly ConcurrentBag<Line> _lines;
        private int _vertexCount;
        private readonly ConcurrentBag<Tuple<string, Vector4, Vector3, Quaternion, Vector3>> _textLines;

        public OpenGlDebugDrawer(in DefaultDebugComponents defaultDebugComponents)
        {
            DebugGeometryShaderProgram = defaultDebugComponents.DebugGeometryShaderProgram;
            DebugLinesShaderProgram = defaultDebugComponents.DebugLinesShaderProgram;
            DebugTextShaderProgram = defaultDebugComponents.DebugTextShaderProgram;
            DebugTextDrawer = defaultDebugComponents.DebugTextDrawer;

            _lines = new ConcurrentBag<Line>();
            _textLines = new ConcurrentBag<Tuple<string, Vector4, Vector3, Quaternion, Vector3>>();
        }

        ~OpenGlDebugDrawer()
        {
            Destroy();
        }

        public void Destroy()
        {
            Unload();

            Destroyed = true;
        }

        public void Draw(Matrix4 viewMatrix, Matrix4 projectionMatrix)
        {
            if (_isBuffering || !Active) return;

            if (_lines.Count > 0)
            {
                BufferVertices();
            }

            DrawLines(viewMatrix, projectionMatrix);
            DrawText(viewMatrix, projectionMatrix);
        }

        private void DrawLines(Matrix4 viewMatrix, Matrix4 projectionMatrix)
        {
            if (DebugLinesShaderProgram == null || !DebugLinesShaderProgram.Active) return;

            DebugLinesShaderProgram.Bind();
            DebugLinesShaderProgram.SetVariable(OpenGlConstants.Uniforms.Model, Matrix4.Identity);
            DebugLinesShaderProgram.SetVariable(OpenGlConstants.Uniforms.View, viewMatrix);
            DebugLinesShaderProgram.SetVariable(OpenGlConstants.Uniforms.Projection, projectionMatrix);

            GL.BindVertexArray(_vertexArrayHandle);

            GL.DrawArrays(BeginMode.Lines, 0, _vertexCount);

            GL.BindVertexArray(0);

            DebugLinesShaderProgram.Unbind();
        }

        private void DrawText(Matrix4 viewMatrix, Matrix4 projectionMatrix)
        {
            if (DebugTextDrawer == null || !DebugTextDrawer.Active) return;

            int textLineCount = _textLines.Count;
            int i = 0;
            while (i < textLineCount)
            {
                if (!_textLines.TryTake(out Tuple<string, Vector4, Vector3, Quaternion, Vector3> line)) continue;

                DebugTextDrawer.Text = line.Item1;
                DebugTextDrawer.Transform.Position = line.Item3;
                DebugTextDrawer.Transform.Rotation = line.Item4;
                DebugTextDrawer.Transform.Scale = line.Item5;
                DebugTextDrawer.Color = line.Item2;

                DebugTextDrawer.Draw(viewMatrix, projectionMatrix);

                i++;
            }
        }

        public void AddLine(Vector3 fromPosition, Vector3 toPosition, Vector4 color)
        {
            if (DebugLinesShaderProgram == null) return;

            _lines.Add(new Line(fromPosition, toPosition, color));
        }

        public void AddWireframeSquare(Vector3 position, Quaternion rotation, Vector3 scale, Vector4 color)
        {
            if (DebugLinesShaderProgram == null) return;

            Matrix4 boxTransformMatrix = new Transform(position, rotation, scale).TransformMatrix;

            Vector4 topLeftBack = new Vector4(-1f, 1f, 0f, 1f) * boxTransformMatrix;
            Vector4 topRightBack = new Vector4(1f, 1f, 0f, 1f) * boxTransformMatrix;
            Vector4 bottomRightBack = new Vector4(1f, -1f, 0f, 1f) * boxTransformMatrix;
            Vector4 bottomLeftBack = new Vector4(-1f, -1f, 0f, 1f) * boxTransformMatrix;

            AddLine(topLeftBack.Xyz, topRightBack.Xyz, color);
            AddLine(topRightBack.Xyz, bottomRightBack.Xyz, color);
            AddLine(bottomRightBack.Xyz, bottomLeftBack.Xyz, color);
            AddLine(bottomLeftBack.Xyz, topLeftBack.Xyz, color);
        }

        public void AddWireframeBox(Vector3 position, Quaternion rotation, Vector3 scale, Vector4 color)
        {
            if (DebugLinesShaderProgram == null) return;

            Matrix4 boxTransformMatrix = new Transform(position, rotation, scale).TransformMatrix;

            Vector4 topLeftBack = new Vector4(-1f, 1f, -1f, 1f) * boxTransformMatrix;
            Vector4 topRightBack = new Vector4(1f, 1f, -1f, 1f) * boxTransformMatrix;
            Vector4 bottomRightBack = new Vector4(1f, -1f, -1f, 1f) * boxTransformMatrix;
            Vector4 bottomLeftBack = new Vector4(-1f, -1f, -1f, 1f) * boxTransformMatrix;

            AddLine(topLeftBack.Xyz, topRightBack.Xyz, color);
            AddLine(topRightBack.Xyz, bottomRightBack.Xyz, color);
            AddLine(bottomRightBack.Xyz, bottomLeftBack.Xyz, color);
            AddLine(bottomLeftBack.Xyz, topLeftBack.Xyz, color);

            Vector4 topLeftFront = new Vector4(-1f, 1f, 1f, 1f) * boxTransformMatrix;
            Vector4 topRightFront = new Vector4(1f, 1f, 1f, 1f) * boxTransformMatrix;
            Vector4 bottomRightFront = new Vector4(1f, -1f, 1f, 1f) * boxTransformMatrix;
            Vector4 bottomLeftFront = new Vector4(-1f, -1f, 1f, 1f) * boxTransformMatrix;

            AddLine(topLeftFront.Xyz, topRightFront.Xyz, color);
            AddLine(topRightFront.Xyz, bottomRightFront.Xyz, color);
            AddLine(bottomRightFront.Xyz, bottomLeftFront.Xyz, color);
            AddLine(bottomLeftFront.Xyz, topLeftFront.Xyz, color);

            AddLine(topLeftBack.Xyz, topLeftFront.Xyz, color);
            AddLine(topRightBack.Xyz, topRightFront.Xyz, color);
            AddLine(bottomRightBack.Xyz, bottomRightFront.Xyz, color);
            AddLine(bottomLeftBack.Xyz, bottomLeftFront.Xyz, color);
        }

        public void AddText(Vector3 position, Quaternion rotation, Vector3 scale, Vector4 color, string text)
        {
            if (DebugTextDrawer == null || string.IsNullOrEmpty(text)) return;

            _textLines.Add(new Tuple<string, Vector4, Vector3, Quaternion, Vector3>(text, color, position, rotation, scale));
        }

        public void AddCrosshair(Vector3 position, Quaternion rotation, Vector3 scale)
        {
            if (DebugLinesShaderProgram == null) return;

            Matrix4 transformMatrix = new Transform(position, rotation, scale).TransformMatrix;

            Vector4 left = new Vector4(-1f, 0f, 0f, 1f) * transformMatrix;
            Vector4 right = new Vector4(1f, 0f, 0f, 1f) * transformMatrix;
            Vector4 top = new Vector4(0f, 1f, 0f, 1f) * transformMatrix;
            Vector4 bottom = new Vector4(0f, -1f, 0f, 1f) * transformMatrix;
            Vector4 front = new Vector4(0f, 0f, 1f, 1f) * transformMatrix;
            Vector4 back = new Vector4(0f, 0f, -1f, 1f) * transformMatrix;

            // X Crosshair
            _lines.Add(new Line(left.Xyz, right.Xyz, new Vector4(1f, 0f, 0f, 1f)));

            // Y Crosshair
            _lines.Add(new Line(bottom.Xyz, top.Xyz, new Vector4(0f, 0f, 1f, 1f)));

            // Z Crosshair
            _lines.Add(new Line(back.Xyz, front.Xyz, new Vector4(0f, 1f, 0f, 1f)));
        }

        public void AddPlanarGrid(Vector3 position, Quaternion rotation, Vector3 scale, int distance)
        {
            Matrix4 transformMatrix = new Transform(position, rotation, scale).TransformMatrix;

            for (int i = -distance; i <= distance; i++)
            {
                // X Division
                Vector4 xLineFrom = new Vector4(i, 0f, -distance, 1f) * transformMatrix;
                Vector4 xLineTo = new Vector4(i, 0f, distance, 1f) * transformMatrix;
                AddLine(xLineFrom.Xyz, xLineTo.Xyz, new Vector4(1f, 0f, 0f, 0.8f));

//                // Y Division
//                Vector3 yLineFrom = new Vector3(-50f, i, 0f);
//                Vector3 yLineTo = new Vector3(50f, i, 0f);
//                AddLine(yLineFrom, yLineTo, new Vector4(0f, 0f, 1f, 0.5f));

                // Z Division
                Vector4 zLineFrom = new Vector4(-distance, 0f, i, 1f) * transformMatrix;
                Vector4 zLineTo = new Vector4(distance, 0f, i, 1f) * transformMatrix;
                AddLine(zLineFrom.Xyz, zLineTo.Xyz, new Vector4(0f, 0f, 1f, 0.8f));
            }
        }

        private void Load()
        {
            if (DebugLinesShaderProgram == null) return;

            GameEngine.RenderEngine.EngineThread.ExecuteOnThread(() =>
            {
                _vertexArrayHandle = GL.GenVertexArray();
                _vertexBufferHandle = GL.GenBuffer();

                BufferVertices(true);

                if (GameEngine.RenderEngine.IsEmbedded)
                {
                    // Position
                    GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, VertexColor.Size, IntPtr.Zero);
                    GL.EnableVertexAttribArray(0);

                    // Color
                    GL.VertexAttribPointer(1, 4, VertexAttribPointerType.Float, false, VertexColor.Size, new IntPtr(3 * sizeof(float)));
                    GL.EnableVertexAttribArray(1);
                }
                else
                {
                    // Position
                    GL.VertexAttribBinding(0, 0);
                    GL.EnableVertexAttribArray(0);
                    GL.VertexAttribFormat(0, 3, VertexAttribFormat.Float, false, 0);

                    // Color
                    GL.VertexAttribBinding(1, 0);
                    GL.EnableVertexAttribArray(1);
                    GL.VertexAttribFormat(1, 4, VertexAttribFormat.Float, false, 3 * sizeof(float));

                    GL.BindVertexBuffer(0, _vertexBufferHandle, IntPtr.Zero, new IntPtr(VertexColor.Size));
                }

                GL.BindVertexArray(0);
                GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

                if (!DebugLinesShaderProgram.Active)
                {
                    DebugLinesShaderProgram.Active = true;
                }
                if (DebugGeometryShaderProgram != null && !DebugGeometryShaderProgram.Active)
                {
                    DebugGeometryShaderProgram.Active = true;
                }
                if (DebugTextDrawer != null && !DebugTextDrawer.Active)
                {
                    DebugTextDrawer.Active = true;
                }
            });
        }

        private void Unload()
        {
            if (_vertexArrayHandle < 1) return;

            _isBuffering = true;
            GameEngine.RenderEngine.EngineThread.ExecuteOnThreadAsync(() =>
            {
                GL.DeleteVertexArray(_vertexArrayHandle);
                _vertexArrayHandle = 0;
                GL.DeleteBuffer(_vertexBufferHandle);
                _vertexBufferHandle = 0;

                _isBuffering = false;
            }).Wait();
        }

        private void BufferVertices(bool leaveOpen = false)
        {
            if (_isBuffering) return;

            _isBuffering = true;
            GameEngine.RenderEngine.EngineThread.ExecuteOnThread(() =>
            {
                GL.BindVertexArray(_vertexArrayHandle);

                GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferHandle);

                List<Line> lines = new List<Line>();

                int lineCount = _lines.Count;
                int i = 0;
                while (i < lineCount)
                {
                    if (!_lines.TryTake(out Line line)) continue;

                    lines.Add(line);

                    i++;
                }

                VertexColor[] vertices = new VertexColor[(lines.Count) * 2];

                int j = 0;
                foreach (Line line in lines)
                {
                    vertices[j] = line.InitVertex;
                    vertices[j + 1] = line.EndVertex;
                    j += 2;
                }

                _vertexCount = lines.Count * 2;

                GL.BufferData(BufferTarget.ArrayBuffer, VertexColor.Size * vertices.Length, ref vertices, BufferUsageHint.DynamicDraw);

                if (leaveOpen)
                {
                    _isBuffering = false;
                    return;
                }

                GL.BindVertexArray(0);
                GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

                _isBuffering = false;
            });
        }
    }
}