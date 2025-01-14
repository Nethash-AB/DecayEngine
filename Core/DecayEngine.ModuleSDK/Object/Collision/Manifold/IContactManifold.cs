using System.Collections.Generic;
using DecayEngine.ModuleSDK.Component.Collision;

namespace DecayEngine.ModuleSDK.Object.Collision.Manifold
{
    public interface IContactManifold
    {
        ICollisionObject CollisionObjectA { get; }
        ICollisionObject CollisionObjectB { get; }
        IEnumerable<IContactManifoldPoint> ContactPoints { get; }
    }
}