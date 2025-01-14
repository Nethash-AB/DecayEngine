using System.Collections.Generic;
using DecayEngine.DecPakLib.Math.Vector;
using DecayEngine.ModuleSDK.Math;

namespace DecayEngine.ModuleSDK.Object.Collision.Shape.Convex.Polyhedral
{
    public interface ICollisionShapePolyhedral : ICollisionShapeConvex
    {
        IEnumerable<Edge> Edges { get; }
        IEnumerable<PlaneNormal> Planes { get; }
        IEnumerable<Vector3> Vertices { get; }
        // ToDo: Implement Faces.

        bool IsInside(Vector3 point, float tolerance);
    }
}