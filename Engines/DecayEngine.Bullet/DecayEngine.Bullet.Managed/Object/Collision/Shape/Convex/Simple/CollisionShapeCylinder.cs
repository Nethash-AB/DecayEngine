using DecayEngine.DecPakLib.Math.Vector;
using DecayEngine.ModuleSDK;
using DecayEngine.ModuleSDK.Object.Collision.Shape.Convex.Simple;
using static DecayEngine.Bullet.Managed.BulletInterop.BulletPhysics;

namespace DecayEngine.Bullet.Managed.Object.Collision.Shape.Convex.Simple
{
    public class CollisionShapeCylinder : CollisionShapeConvex, ICollisionShapeCylinder
    {
        public Vector3 HalfExtents
        {
            get
            {
                if (!IsNativeHandleInitialized) return Vector3.Zero;

                return GameEngine.PhysicsEngine.EngineThread.ExecuteOnThread(() =>
                {
                    btCylinderShape_getHalfExtentsWithoutMargin(NativeHandle, out Vector3 val);
                    return val;
                });
            }
        }

        public float Radius => !IsNativeHandleInitialized ?
            0f :
            GameEngine.PhysicsEngine.EngineThread.ExecuteOnThread(() => btCapsuleShape_getRadius(NativeHandle));

        public CollisionShapeCylinder(Vector3 halfExtents)
        {
            GameEngine.PhysicsEngine.EngineThread.ExecuteOnThread(() =>
            {
                NativeHandle = btCylinderShape_new(ref halfExtents);
            });
        }

        public CollisionShapeCylinder(float halfExtentX, float halfExtentY, float halfExtentZ)
        {
            GameEngine.PhysicsEngine.EngineThread.ExecuteOnThread(() =>
            {
                NativeHandle = btCylinderShape_new2(halfExtentX, halfExtentY, halfExtentZ);
            });
        }
    }
}