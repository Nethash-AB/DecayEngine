using System.Runtime.InteropServices;
using DecayEngine.DecPakLib.Math.Vector;

namespace DecayEngine.OpenGL.Object.BufferStructure
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct VertexColor
    {
        public static int Size => Vector3.SizeInBytes + Vector4.SizeInBytes;

        private Vector3 _position;
        private Vector4 _color;

        public Vector3 Position
        {
            get => _position;
            set => _position = value;
        }

        public Vector4 Color
        {
            get => _color;
            set => _color = value;
        }

        public VertexColor(Vector3 position, Vector4 color)
        {
            _position = position;
            _color = color;
        }
    }
}