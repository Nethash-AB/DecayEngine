using System.Runtime.InteropServices;
using DecayEngine.DecPakLib.Math.Vector;

namespace DecayEngine.OpenGL.Object.BufferStructure
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Line
    {
        public VertexColor InitVertex { get; }
        public VertexColor EndVertex { get; }

        public Line(Vector3 fromPosition, Vector3 toPosition, Vector4 color)
        {
            InitVertex = new VertexColor(fromPosition, color);
            EndVertex = new VertexColor(toPosition, color);
        }
    }
}