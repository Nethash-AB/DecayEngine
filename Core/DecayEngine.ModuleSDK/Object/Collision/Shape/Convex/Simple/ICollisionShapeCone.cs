namespace DecayEngine.ModuleSDK.Object.Collision.Shape.Convex.Simple
{
    public interface ICollisionShapeCone : ICollisionShapeConvex
    {
        float Height { get; set; }
        float Radius { get; set; }
    }
}