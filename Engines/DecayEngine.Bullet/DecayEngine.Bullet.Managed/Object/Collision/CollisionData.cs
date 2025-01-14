using System.Collections.Generic;
using DecayEngine.DecPakLib.Math.Vector;
using DecayEngine.ModuleSDK.Component.Collision;
using DecayEngine.ModuleSDK.Component.RigidBody;
using DecayEngine.ModuleSDK.Object.Collision;

namespace DecayEngine.Bullet.Managed.Object.Collision
{
    public class CollisionData : ICollisionData
    {
        public ICollisionObject CollisionObjectSelf { get; }
        public ICollisionObject CollisionObjectOther { get; }
        public IEnumerable<Vector3> ContactPointsSelf { get; }
        public IEnumerable<Vector3> ContactPointsOther { get; }

        public CollisionData(ICollisionObject selfObject, ICollisionObject otherObject)
        {
            CollisionObjectSelf = selfObject;
            CollisionObjectOther = otherObject;
            ContactPointsSelf = new List<Vector3>();
            ContactPointsOther = new List<Vector3>();
        }
    }
}