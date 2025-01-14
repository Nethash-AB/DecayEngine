using System.Collections.Generic;
using DecayEngine.ModuleSDK.Component.Collision;
using DecayEngine.ModuleSDK.Object.Collision.World;

namespace DecayEngine.ModuleSDK.Object.Collision.Filter
{
    public interface ICollisionGroup
    {
        IPhysicsWorld PhysicsWorld { get; }
        string Name { get; }
        IEnumerable<ICollisionObject> CollisionObjects { get; }
        IEnumerable<ICollisionGroup> IgnoredGroups { get; }
    }
}