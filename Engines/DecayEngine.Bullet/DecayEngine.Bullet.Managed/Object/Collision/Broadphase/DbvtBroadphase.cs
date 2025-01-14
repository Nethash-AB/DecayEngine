using System;
using System.Linq;
using System.Runtime.InteropServices;
using DecayEngine.Bullet.Managed.BulletInterop;
using DecayEngine.Bullet.Managed.Component.Collision;
using DecayEngine.ModuleSDK;
using static DecayEngine.Bullet.Managed.BulletInterop.BulletPhysics;

namespace DecayEngine.Bullet.Managed.Object.Collision.Broadphase
{
    public class DbvtBroadphase : NativeObject
    {
        private readonly NeedBroadphaseCollisionUnmanagedDelegate _needCollisionDelegate;
        private IntPtr _overlapFilterCallback;

        public DbvtBroadphase()
        {
            _needCollisionDelegate = NeedsCollision;

            GameEngine.PhysicsEngine.EngineThread.ExecuteOnThread(() =>
            {
                NativeHandle = btDbvtBroadphase_new(IntPtr.Zero);
                _overlapFilterCallback = btOverlapFilterCallbackWrapper_new(Marshal.GetFunctionPointerForDelegate(_needCollisionDelegate));

                IntPtr overlappingPairCacheHandle = btBroadphaseInterface_getOverlappingPairCache(NativeHandle);
                btOverlappingPairCache_setOverlapFilterCallback(overlappingPairCacheHandle, _overlapFilterCallback);
            });
        }

        private static bool NeedsCollision(IntPtr proxyHandleA, IntPtr proxyHandleB)
        {
            CollisionObject collisionObjectA = CollisionObject.FromNativeBroadphaseHandle(proxyHandleA);
            if (collisionObjectA == null) return false;

            CollisionObject collisionObjectB = CollisionObject.FromNativeBroadphaseHandle(proxyHandleB);
            if (collisionObjectB == null) return false;

            return
                collisionObjectA.PhysicsWorld == collisionObjectB.PhysicsWorld &&
                collisionObjectA.CollisionGroup != null && collisionObjectB.CollisionGroup != null &&
                !collisionObjectA.CollisionGroup.IgnoredGroups.Contains(collisionObjectB.CollisionGroup) &&
                !collisionObjectA.IgnoredCollisionObjects.Contains(collisionObjectB) &&
                (collisionObjectA.CollisionFilter == null || collisionObjectA.CollisionFilter(collisionObjectB)) &&
                (collisionObjectB.CollisionFilter == null || collisionObjectB.CollisionFilter(collisionObjectA));
        }

        protected override void FreeUnmanagedHandles()
        {
            if (!IsNativeHandleInitialized) return;

            GameEngine.PhysicsEngine.EngineThread.ExecuteOnThread(() =>
            {
                btBroadphaseInterface_delete(NativeHandle);
                btOverlapFilterCallback_delete(_overlapFilterCallback);
            });
        }
    }
}