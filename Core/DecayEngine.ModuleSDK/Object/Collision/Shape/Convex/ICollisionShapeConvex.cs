using DecayEngine.DecPakLib.Math.Vector;

namespace DecayEngine.ModuleSDK.Object.Collision.Shape.Convex
{
    public interface ICollisionShapeConvex : ICollisionShape
    {
        Vector3 ImplicitDimensions { get; set; }
    }
}