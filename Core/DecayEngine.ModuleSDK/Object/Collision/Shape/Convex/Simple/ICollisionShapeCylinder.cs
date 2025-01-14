using DecayEngine.DecPakLib.Math.Vector;

namespace DecayEngine.ModuleSDK.Object.Collision.Shape.Convex.Simple
{
    public interface ICollisionShapeCylinder : ICollisionShapeConvex
    {
        Vector3 HalfExtents { get; }
        float Radius { get; }
    }
}