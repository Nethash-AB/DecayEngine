using DecayEngine.ModuleSDK;
using DecayEngine.ModuleSDK.Object.Collision.Shape.Convex.Simple;
using static DecayEngine.Bullet.Managed.BulletInterop.BulletPhysics;

namespace DecayEngine.Bullet.Managed.Object.Collision.Shape.Convex.Simple
{
    public class CollisionShapeSphere : CollisionShapeConvex, ICollisionShapeSphere
    {
        public float Radius
        {
            get => !IsNativeHandleInitialized ? 0f : GameEngine.PhysicsEngine.EngineThread.ExecuteOnThread(() => ImplicitDimensions.X);
            set
            {
                if (!IsNativeHandleInitialized) return;
                GameEngine.PhysicsEngine.EngineThread.ExecuteOnThread(() => btSphereShape_setUnscaledRadius(NativeHandle, value));
            }
        }

        public CollisionShapeSphere(float radius)
        {
            GameEngine.PhysicsEngine.EngineThread.ExecuteOnThread(() =>
            {
                NativeHandle = btSphereShape_new(radius);
            });
        }
    }
}