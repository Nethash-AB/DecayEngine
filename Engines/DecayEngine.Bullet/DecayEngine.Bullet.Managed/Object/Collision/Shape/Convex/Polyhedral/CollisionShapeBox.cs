using DecayEngine.DecPakLib.Math.Vector;
using DecayEngine.ModuleSDK;
using DecayEngine.ModuleSDK.Object.Collision.Shape.Convex.Polyhedral;
using static DecayEngine.Bullet.Managed.BulletInterop.BulletPhysics;

namespace DecayEngine.Bullet.Managed.Object.Collision.Shape.Convex.Polyhedral
{
    public class CollisionShapeBox : CollisionShapePolyhedral, ICollisionShapeBox
    {
        public Vector3 HalfExtents
        {
            get
            {
                if (!IsNativeHandleInitialized) return Vector3.Zero;

                return GameEngine.PhysicsEngine.EngineThread.ExecuteOnThread(() =>
                {
                    btBoxShape_getHalfExtentsWithoutMargin(NativeHandle, out Vector3 val);
                    return val;
                });
            }
        }

        public CollisionShapeBox(Vector3 halfExtents)
        {
            GameEngine.PhysicsEngine.EngineThread.ExecuteOnThread(() =>
            {
                NativeHandle = btBoxShape_new(ref halfExtents);
            });
        }

        public CollisionShapeBox(float halfExtent)
        {
            GameEngine.PhysicsEngine.EngineThread.ExecuteOnThread(() =>
            {
                NativeHandle = btBoxShape_new2(halfExtent);
            });
        }

        public CollisionShapeBox(float halfExtentX, float halfExtentY, float halfExtentZ)
        {
            GameEngine.PhysicsEngine.EngineThread.ExecuteOnThread(() =>
            {
                NativeHandle = btBoxShape_new3(halfExtentX, halfExtentY, halfExtentZ);
            });
        }
    }
}