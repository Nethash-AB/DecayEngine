using System.Runtime.InteropServices;
using DecayEngine.DecPakLib.Math.Vector;

namespace DecayEngine.OpenGL.Object.BufferStructure
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct VertexPbr
    {
        public static int Size => Vector3.SizeInBytes * 4 + Vector2.SizeInBytes;

        private Vector3 _position;
        private Vector3 _normal;
        private Vector3 _tangent;
        private Vector3 _bitangent;
        private Vector2 _textureUv;

        public Vector3 Position
        {
            get => _position;
            set => _position = value;
        }

        public Vector3 Normal
        {
            get => _normal;
            set => _normal = value;
        }

        public Vector3 Tangent
        {
            get => _tangent;
            set => _tangent = value;
        }

        public Vector3 Bitangent
        {
            get => _bitangent;
            set => _bitangent = value;
        }

        public Vector2 TextureUv
        {
            get => _textureUv;
            set => _textureUv = value;
        }

        public VertexPbr(Vector3 position, Vector3 normal, Vector3 tangent, Vector3 bitangent, Vector2 textureUv)
        {
            _position = position;
            _normal = normal;
            _tangent = tangent;
            _bitangent = bitangent;
            _textureUv = textureUv;
        }
    }
}