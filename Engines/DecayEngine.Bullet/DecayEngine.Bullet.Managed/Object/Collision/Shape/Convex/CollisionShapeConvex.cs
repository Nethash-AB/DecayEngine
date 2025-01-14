using DecayEngine.DecPakLib.Math.Vector;
using DecayEngine.ModuleSDK;
using DecayEngine.ModuleSDK.Object.Collision.Shape.Convex;
using static DecayEngine.Bullet.Managed.BulletInterop.BulletPhysics;

namespace DecayEngine.Bullet.Managed.Object.Collision.Shape.Convex
{
    public abstract class CollisionShapeConvex : CollisionShape, ICollisionShapeConvex
    {
        public Vector3 ImplicitDimensions
        {
            get
            {
                if (!IsNativeHandleInitialized) return Vector3.Zero;

                return GameEngine.PhysicsEngine.EngineThread.ExecuteOnThread(() =>
                {
                    btConvexInternalShape_getImplicitShapeDimensions(NativeHandle, out Vector3 val);
                    return val;
                });
            }
            set
            {
                if (!IsNativeHandleInitialized) return;
                GameEngine.PhysicsEngine.EngineThread.ExecuteOnThread(() => btConvexInternalShape_setImplicitShapeDimensions(NativeHandle, ref value));
            }
        }
    }
}