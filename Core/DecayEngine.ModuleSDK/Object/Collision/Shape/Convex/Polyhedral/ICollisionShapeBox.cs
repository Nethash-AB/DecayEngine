using DecayEngine.DecPakLib.Math.Vector;

namespace DecayEngine.ModuleSDK.Object.Collision.Shape.Convex.Polyhedral
{
    public interface ICollisionShapeBox : ICollisionShapePolyhedral
    {
        Vector3 HalfExtents { get; }
    }
}