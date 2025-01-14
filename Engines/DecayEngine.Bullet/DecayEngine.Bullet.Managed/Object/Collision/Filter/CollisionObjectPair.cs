using DecayEngine.ModuleSDK.Component.Collision;
using DecayEngine.ModuleSDK.Object.Collision;
using DecayEngine.ModuleSDK.Object.Collision.Filter;

namespace DecayEngine.Bullet.Managed.Object.Collision.Filter
{
    public class CollisionObjectPair : ICollisionObjectPair
    {
        public ICollisionObject CollisionObjectA { get; }
        public ICollisionObject CollisionObjectB { get; }

        public CollisionObjectPair(ICollisionObject collisionObjectA, ICollisionObject collisionObjectB)
        {
            CollisionObjectA = collisionObjectA;
            CollisionObjectB = collisionObjectB;
        }
    }
}