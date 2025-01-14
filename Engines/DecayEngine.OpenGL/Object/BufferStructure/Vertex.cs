using System.Runtime.InteropServices;
using DecayEngine.DecPakLib.Math.Vector;

namespace DecayEngine.OpenGL.Object.BufferStructure
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Vertex
    {
        public static int Size => Vector3.SizeInBytes + Vector2.SizeInBytes;

        private Vector3 _position;
        private Vector2 _textureUv;

        public Vector3 Position
        {
            get => _position;
            set => _position = value;
        }

        public Vector2 TextureUv
        {
            get => _textureUv;
            set => _textureUv = value;
        }

        public Vertex(Vector3 position, Vector2 textureUv)
        {
            _position = position;
            _textureUv = textureUv;
        }

        public Vertex(float x, float y, float z, float u, float v) : this(new Vector3(x, y, z), new Vector2(u, v))
        {
        }

        public Vertex(float x, float y, float z) : this(new Vector3(x, y, z), Vector2.Zero)
        {
        }
    }
}