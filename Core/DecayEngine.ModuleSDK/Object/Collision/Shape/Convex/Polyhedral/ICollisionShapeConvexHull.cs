using System.Collections.Generic;
using DecayEngine.DecPakLib.Math.Vector;

namespace DecayEngine.ModuleSDK.Object.Collision.Shape.Convex.Polyhedral
{
    public interface ICollisionShapeConvexHull : ICollisionShapePolyhedral
    {
        IEnumerable<Vector3> Points { get; }

        void AddPoint(Vector3 point, bool recalculateLocalAabb = true);
        void Optimize();
        void RecalculateLocalAabb();
    }
}