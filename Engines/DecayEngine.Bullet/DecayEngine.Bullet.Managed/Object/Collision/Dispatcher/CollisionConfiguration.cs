using System;
using DecayEngine.Bullet.Managed.BulletInterop;
using DecayEngine.ModuleSDK;
using static DecayEngine.Bullet.Managed.BulletInterop.BulletPhysics;

namespace DecayEngine.Bullet.Managed.Object.Collision.Dispatcher
{
    public class CollisionConfiguration : NativeObject
    {
        public CollisionConfiguration()
        {
            GameEngine.PhysicsEngine.EngineThread.ExecuteOnThread(() =>
            {
                NativeHandle = btDefaultCollisionConfiguration_new();
            });
        }

        protected override void FreeUnmanagedHandles()
        {
            if (!IsNativeHandleInitialized) return;

            GameEngine.PhysicsEngine.EngineThread.ExecuteOnThread(() =>
            {
                btCollisionConfiguration_delete(NativeHandle);
            });
        }
    }
}