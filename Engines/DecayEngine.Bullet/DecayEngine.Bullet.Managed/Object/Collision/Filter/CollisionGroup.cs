using System.Collections.Generic;
using System.Linq;
using DecayEngine.ModuleSDK.Component.Collision;
using DecayEngine.ModuleSDK.Object.Collision;
using DecayEngine.ModuleSDK.Object.Collision.Filter;
using DecayEngine.ModuleSDK.Object.Collision.World;

namespace DecayEngine.Bullet.Managed.Object.Collision.Filter
{
    public class CollisionGroup : ICollisionGroup
    {
        public IPhysicsWorld PhysicsWorld { get; }
        public string Name { get; }

        public IEnumerable<ICollisionObject> CollisionObjects =>
            PhysicsWorld == null
                ? new List<ICollisionObject>()
                : PhysicsWorld.CollisionObjects
                    .Where(co => co.CollisionGroup == this);

        public IEnumerable<ICollisionGroup> IgnoredGroups =>
            PhysicsWorld == null
                ? new List<ICollisionGroup>()
                : PhysicsWorld.IgnoredCollisionGroupPairs
                    .Where(pair => pair.CollisionGroupA == this || pair.CollisionGroupB == this)
                    .Select(pair => pair.CollisionGroupA == this ? pair.CollisionGroupB : pair.CollisionGroupA);

        public CollisionGroup(string name, IPhysicsWorld physicsWorld)
        {
            Name = name;
            PhysicsWorld = physicsWorld;
        }
    }
}