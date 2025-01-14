using DecayEngine.ModuleSDK;
using DecayEngine.ModuleSDK.Object.Collision.Shape.Convex.Simple;
using static DecayEngine.Bullet.Managed.BulletInterop.BulletPhysics;

namespace DecayEngine.Bullet.Managed.Object.Collision.Shape.Convex.Simple
{
    public class CollisionShapeCapsule : CollisionShapeConvex, ICollisionShapeCapsule
    {
        public float Height => !IsNativeHandleInitialized ?
            0f :
            GameEngine.PhysicsEngine.EngineThread.ExecuteOnThread(() => btCapsuleShape_getHalfHeight(NativeHandle) * 2f);

        public float Radius => !IsNativeHandleInitialized ?
            0f :
            GameEngine.PhysicsEngine.EngineThread.ExecuteOnThread(() => btCapsuleShape_getRadius(NativeHandle));

        public CollisionShapeCapsule(float radius, float height)
        {
            GameEngine.PhysicsEngine.EngineThread.ExecuteOnThread(() =>
            {
                NativeHandle = btCapsuleShape_new(radius, height);
            });
        }
    }
}