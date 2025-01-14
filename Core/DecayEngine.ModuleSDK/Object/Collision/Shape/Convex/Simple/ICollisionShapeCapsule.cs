namespace DecayEngine.ModuleSDK.Object.Collision.Shape.Convex.Simple
{
    public interface ICollisionShapeCapsule : ICollisionShapeConvex
    {
        float Height { get; }
        float Radius { get; }
    }
}