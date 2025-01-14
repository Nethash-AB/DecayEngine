using DecayEngine.ModuleSDK.Component.Collision;

namespace DecayEngine.ModuleSDK.Object.Collision.Filter
{
    public interface ICollisionObjectPair
    {
        ICollisionObject CollisionObjectA { get; }
        ICollisionObject CollisionObjectB { get; }
    }
}