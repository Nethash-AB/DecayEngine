using DecayEngine.DecPakLib.Math.Vector;

namespace DecayEngine.ModuleSDK.Math
{
    public class Edge
    {
        public Vector3 PointA { get; set; }
        public Vector3 PointB { get; set; }

        public Edge(Vector3 pointA, Vector3 pointB)
        {
            PointA = pointA;
            PointB = pointB;
        }
    }
}