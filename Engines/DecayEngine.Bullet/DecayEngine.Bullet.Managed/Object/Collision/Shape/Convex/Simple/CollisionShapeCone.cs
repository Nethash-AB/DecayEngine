using DecayEngine.ModuleSDK;
using DecayEngine.ModuleSDK.Object.Collision.Shape.Convex.Simple;
using static DecayEngine.Bullet.Managed.BulletInterop.BulletPhysics;

namespace DecayEngine.Bullet.Managed.Object.Collision.Shape.Convex.Simple
{
    public class CollisionShapeCone : CollisionShapeConvex, ICollisionShapeCone
    {
        public float Height
        {
            get => !IsNativeHandleInitialized ? 0f : GameEngine.PhysicsEngine.EngineThread.ExecuteOnThread(() => btConeShape_getHeight(NativeHandle));
            set
            {
                if (!IsNativeHandleInitialized) return;
                GameEngine.PhysicsEngine.EngineThread.ExecuteOnThread(() => btConeShape_setHeight(NativeHandle, value));
            }
        }

        public float Radius
        {
            get => !IsNativeHandleInitialized ? 0f : GameEngine.PhysicsEngine.EngineThread.ExecuteOnThread(() => btConeShape_getRadius(NativeHandle));
            set
            {
                if (!IsNativeHandleInitialized) return;
                GameEngine.PhysicsEngine.EngineThread.ExecuteOnThread(() => btConeShape_setRadius(NativeHandle, value));
            }
        }

        public CollisionShapeCone(float radius, float height)
        {
            GameEngine.PhysicsEngine.EngineThread.ExecuteOnThread(() =>
            {
                NativeHandle = btConeShape_new(radius, height);
            });
        }
    }
}