using DecayEngine.ModuleSDK.Object.Collision.Filter;

namespace DecayEngine.Bullet.Managed.Object.Collision.Filter
{
    public class CollisionGroupPair : ICollisionGroupPair
    {
        public ICollisionGroup CollisionGroupA { get; }
        public ICollisionGroup CollisionGroupB { get; }

        public CollisionGroupPair(ICollisionGroup collisionGroupA, ICollisionGroup collisionGroupB)
        {
            CollisionGroupA = collisionGroupA;
            CollisionGroupB = collisionGroupB;
        }
    }
}