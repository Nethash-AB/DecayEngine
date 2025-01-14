using System.Collections.Generic;
using DecayEngine.DecPakLib.Math.Vector;
using DecayEngine.ModuleSDK.Component.Collision;

namespace DecayEngine.ModuleSDK.Object.Collision
{
    public interface ICollisionData
    {
        ICollisionObject CollisionObjectSelf { get; }
        ICollisionObject CollisionObjectOther { get; }
        IEnumerable<Vector3> ContactPointsSelf { get; }
        IEnumerable<Vector3> ContactPointsOther { get; }
    }
}